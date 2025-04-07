using System;

namespace H9e.Core {
    public class H9eRandom {
        private static Random GlobalRandom;
        static H9eRandom() {
            GlobalRandom = CreateRandom();
        }

        public static Random CreateRandom() {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            int iSeed = BitConverter.ToInt32(buffer, 0);
            return new Random(unchecked(~iSeed));
        }

        public static Random GetInstance() {
            return GlobalRandom;
        }


    }
}
