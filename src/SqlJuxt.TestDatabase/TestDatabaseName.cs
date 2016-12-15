using System;
using System.Globalization;

namespace SqlJuxt.TestDatabase
{
    public class TestDatabaseName
    {
        private const string DateFormat = "yyyyMMddHHmmssFF";

        public TestDatabaseName(string domain, TestingType testingType, DateTime createdAt, Guid id)
        {
            Domain = domain;
            TestingType = testingType;
            CreatedAt = createdAt;
            Id = id;
        }

        public TestDatabaseName(string domain, TestingType testingType, DateTime createdAt)
        {
            Domain = domain;
            TestingType = testingType;
            CreatedAt = createdAt;
            Id = Guid.NewGuid();
        }

        public string Domain { get; private set; }
        public TestingType TestingType { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public Guid Id { get; private set; }

        public static implicit operator string(TestDatabaseName name)
        {
            return name.ToString();
        }

        public override string ToString()
        {
            return string.Format("{0}-{1}-{2}-{3}", Domain, TestingType, CreatedAt.ToString(DateFormat), Id.ToString("N"));
        }

        public static bool TryParse(string fullName, out TestDatabaseName testDatabaseName)
        {
            testDatabaseName = null;

            var values = fullName.Split('-');

            if (values.Length < 3)
            {
                return false;
            }

            var domain = values[0];


            TestingType type;
            DateTime createdAt;

            if (!Enum.TryParse(values[1], out type) ||
                !DateTime.TryParseExact(values[2], DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None,
                    out createdAt)) return false;

            var id = Guid.Empty;

            if (values.Length > 3)
            {
                if (!Guid.TryParse(values[3], out id))
                    return false;
            }


            testDatabaseName = new TestDatabaseName(domain, type, createdAt, id);

            return true;
        }
    }
}