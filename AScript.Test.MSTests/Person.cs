using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Test.MSTests
{
	public class Person
	{
		public string Name { get; set; }
		public int Age { get; set; }
		public string Name2
		{
			set => this.Name = value;
		}

		public event EventHandler<EventArgs> Saying;

		public Person() { }
		public Person(string name, int age)
		{
			this.Name = name;
			this.Age = age;
		}

		protected virtual void OnSaying(EventArgs e)
		{
			this.Saying?.Invoke(this, e);
		}

		public string SayHello()
		{
			OnSaying(EventArgs.Empty);
			return $"Hello, my name is {this.Name}, I'm {this.Age} years old";
		}

		public string Play(string game)
		{
			return $"{this.Name} play game {game}";
		}

		public static Person Create(string name, int age)
		{
			return new Person(name, age);
		}
	}
}
