using System;

namespace TestConsole
{
   
    public class A
    {
        public virtual void Func1(int i)
        {
            Console.WriteLine(i);
        }

        public  void Func2(A a)
        {
            a.Func1(1);
            Func1(3);
        }
    }


    public class B : A
    {
        public override void Func1(int i)
        {
            base.Func1(i+1);
        }

        public static void Main()
        {
            B b = new B();
            A a = new A();
            a.Func2(b);
            b.Func2(a);
        }
    }
}


