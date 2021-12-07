using System;

namespace Main
{
    public class User
    {
        public String name;
        public int age;
        public Dog[] dogs;
        public long test { get; set; }
        private float money = 10.5f;
        public Profile profile;
    }

    public class Dog
    {
        public string name;
        public User owner;
        public Dog(string name, User owner)
        {
            this.name = name;
            this.owner = owner;
        }

    }

    public class Profile
    {
        public string address;
        public Profile(string address)
        {
            this.address = address;
        }
        public Profile(string address, string hello)
        {
            this.address = address;
            throw new Exception();
        }
    }
}
