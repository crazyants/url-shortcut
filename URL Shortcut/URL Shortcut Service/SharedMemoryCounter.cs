namespace URL_Shortcut_Service
{
    static class SharedMemoryCounter
    {
        // The uninitialized state for the counter
        private const long ZERO = 0;

        // The counter that feed the async server socket
        private static long counter = ZERO;

        // The lock to control the counter reads and writes
        private static object lockObject = new object();

        public static bool Initialize(long value)
        {
            // Initialize the counter if and only if it has not yet been initialized
            if (counter == ZERO)
            {
                // Lock while initializing the counter
                lock (lockObject)
                {
                    counter = value;
                }

                // Operation successful
                return true;
            }

            // The counter is initialized already
            return false;
        }

        public static long Capture()
        {
            long value;

            // Lock while interacting with the counter
            lock (lockObject)
            {
                // Get counter value
                value = counter;

                // Increase the counter value by one
                ++counter;
            }

            return value;
        }
    }
}
