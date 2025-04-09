namespace H9e.Database.SQLBuilder {
    public class H9eDBPage {
        public int Number { get; set; }
        public int Size { get; set; }

        private H9eDBPage(int number, int size) {
            Number = number;
            Size = size;
        }

        public override string ToString() {
            if (Number < 0) {
                Number = 0;
            }
            if (Size < 0) {
                Size = 1;
            }
            return $"LIMIT {Size} OFFSET {Number * Size}";
        }

        public static H9eDBPage Limit(int number, int size) {
            return new H9eDBPage(number, size);
        }
    }
}
