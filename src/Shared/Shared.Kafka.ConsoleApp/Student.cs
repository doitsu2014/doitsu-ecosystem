using System;
using System.Text.Json;
using Bogus;
using Confluent.Kafka;

namespace Shared.Kafka.ConsoleApp
{
    public class StudentSerializer : ISerializer<Student>, IDeserializer<Student>
    {
        public Student Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            return JsonSerializer.Deserialize<Student>(data);
        }

        public byte[] Serialize(Student data, SerializationContext context)
        {
            return JsonSerializer.SerializeToUtf8Bytes(data);
        }
    }

    public class Student
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public float Gpa { get; set; }
    }

    public static class StudentFaker
    {
        public static Faker<Student> GetFaker() => new Faker<Student>("en")
            .RuleFor(x => x.Id, y => Guid.NewGuid())
            .RuleFor(x => x.Name, y => y.Person.FullName)
            .RuleFor(x => x.Gpa, y => y.Random.Float(5, 10));
    }
}