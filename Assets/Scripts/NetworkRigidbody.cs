using UnityEngine;
using System.Collections;

public class NetworkRigidbody : MonoBehaviour
{
    public int number = -1;

    public double m_InterpolationBackTime = 0.1;
    public double m_ExtrapolationLimit = 0.5;

    internal struct State
    {
        internal float timestamp;
        internal Vector3 pos;
        internal Vector3 velocity;
        internal Quaternion rot;
        internal Vector3 angularVelocity;
    }

    // We store twenty states with "playback" information
    State[] m_BufferedState = new State[20];
    // Keep track of what slots are used
    int m_TimestampCount;

    public MainScript mainScript;

    public float timer = 0.0f;
    public float timeRate = 0.1f;

    void Start()
    {
        mainScript = GameObject.Find("MainScript").GetComponent<MainScript>();
    }

    public void SetPosition(
        int n,
        Vector3 pos,
        Quaternion rot,
        Vector3 velocity,
        Vector3 angularVelocity,
        double time)
    {
        // Shift the buffer sideways, deleting state 20
        for (int i = m_BufferedState.Length - 1; i >= 1; i--)
        {
            m_BufferedState[i] = m_BufferedState[i - 1];
        }

        // Record current state in slot 0
        State state;
        state.timestamp = (float)Network.time;
        state.pos = pos;
        state.velocity = velocity;
        state.rot = rot;
        state.angularVelocity = angularVelocity;

        m_BufferedState[0] = state;

        // Update used slot count, however never exceed the buffer size
        // Slots aren't actually freed so this just makes sure the buffer is
        // filled up and that uninitalized slots aren't used.
        m_TimestampCount = Mathf.Min(m_TimestampCount + 1, m_BufferedState.Length);

        // Check if states are in order, if it is inconsistent you could reshuffel or 
        // drop the out-of-order state. Nothing is done here
        for (int i = 0; i < m_TimestampCount - 1; i++)
        {
            if (m_BufferedState[i].timestamp < m_BufferedState[i + 1].timestamp)
            {
                Debug.Log("State inconsistent");
            }
        }
    }

    // We have a window of interpolationBackTime where we basically play 
    // By having interpolationBackTime the average ping, you will usually use interpolation.
    // And only if no more data arrives we will use extra polation
    void Update()
    {
        if (Network.isServer)
        {
            //if (timer < Time.time)
            //{
            //    timer = Time.time + timeRate;

            //    mainScript.SetPosition(
            //        number,
            //        transform.position,
            //        transform.rotation,
            //        rigidbody.velocity,
            //        rigidbody.angularVelocity,
            //        Time.realtimeSinceStartup);
            //}
        }
        else if (Network.isClient)
        {
            // This is the target playback time of the rigid body
            double interpolationTime = Network.time - m_InterpolationBackTime;

            // Use interpolation if the target playback time is present in the buffer
            if (m_BufferedState[0].timestamp > interpolationTime)
            {
                // Go through buffer and find correct state to play back
                for (int i = 0; i < m_TimestampCount; i++)
                {
                    if (m_BufferedState[i].timestamp <= interpolationTime || i == m_TimestampCount - 1)
                    {
                        // The state one slot newer (<100ms) than the best playback state
                        State rhs = m_BufferedState[Mathf.Max(i - 1, 0)];
                        // The best playback state (closest to 100 ms old (default time))
                        State lhs = m_BufferedState[i];

                        // Use the time between the two slots to determine if interpolation is necessary
                        double length = rhs.timestamp - lhs.timestamp;
                        float t = 0.0f;
                        // As the time difference gets closer to 100 ms t gets closer to 1 in 
                        // which case rhs is only used
                        // Example:
                        // Time is 10.000, so sampleTime is 9.900 
                        // lhs.time is 9.910 rhs.time is 9.980 length is 0.070
                        // t is 9.900 - 9.910 / 0.070 = 0.14. So it uses 14% of rhs, 86% of lhs
                        if (length > 0.0001f)
                        {
                            t = (float)((interpolationTime - lhs.timestamp) / length);
                        }

                        // if t=0 => lhs is used directly
                        Vector3 lerpPosition = Vector3.Lerp(lhs.pos, rhs.pos, t);
                        if (lerpPosition.magnitude > 0.0f)
                        {
                            transform.localPosition = Vector3.Lerp(lhs.pos, rhs.pos, t);
                        }
                        //transform.localRotation = Quaternion.Slerp(lhs.rot, rhs.rot, t);
                        //transform.localRotation = lhs.rot;
                        return;
                    }
                }
                // Use extrapolation
            }
            else
            {
                State latest = m_BufferedState[0];

                float extrapolationLength = (float)(interpolationTime - latest.timestamp);
                // Don't extrapolation for more than 500 ms, you would need to do that carefully
                if (extrapolationLength < m_ExtrapolationLimit)
                {
                    float axisLength = extrapolationLength * latest.angularVelocity.magnitude * Mathf.Rad2Deg;
                    Quaternion angularRotation = Quaternion.AngleAxis(axisLength, latest.angularVelocity);

                    rigidbody.position = latest.pos + latest.velocity * extrapolationLength;
                    rigidbody.rotation = angularRotation * latest.rot;
                    //rigidbody.velocity = latest.velocity;
                    //rigidbody.angularVelocity = latest.angularVelocity;
                }
            }
        }
    }
}
