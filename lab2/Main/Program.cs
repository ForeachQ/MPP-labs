using System;
using Newtonsoft.Json;

namespace Main
{
    class Program
    {
        static void Main(string[] args)
        {
            Faker.Faker f = new Faker.Faker();
            User ff = f.Create<User>();
            //ICollection<int> ff = f.Create<List<int>>();
            Console.WriteLine(JsonConvert.SerializeObject(ff,Formatting.Indented));

            //Console.WriteLine(ff.ToString());
        }
    }
}
