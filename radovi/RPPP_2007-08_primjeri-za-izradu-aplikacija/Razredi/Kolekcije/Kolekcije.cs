using System;
using System.Collections;
using System.Text;

namespace Kolekcije
{
  class Kolekcije
  {
    static void Main(string[] args)
    {

      Console.WriteLine("***** ArrayList *****");

      ArrayList lista = new ArrayList();
      lista.Add(1);
      lista.Add("abc");

      Console.WriteLine("ArrayList:");
      foreach (object o in lista)
      {
        Console.WriteLine(o.ToString());
      }

      if (lista.Contains("abc")) Console.WriteLine("Lista sadr�i abc");

      Console.WriteLine("Lista sadr�i " + lista.Count + " elemenata");
      lista.Clear();
      Console.WriteLine("Nakon brisanja: Lista sadr�i " + lista.Count + " elemenata");

      Console.WriteLine("\n***** Stack *****");

      Stack stog = new Stack();
      stog.Push("abc");
      stog.Push(123);
      Console.WriteLine("Stog sadr�i " + stog.Count.ToString() + " elemenata.");
      Console.WriteLine("S vrha stoga (pop): " + stog.Pop().ToString());
      Console.WriteLine("Stog sadr�i " + stog.Count.ToString() + " elemenata.");
      Console.WriteLine("S vrha stoga (peek): " + stog.Peek().ToString());
      Console.WriteLine("Stog sadr�i " + stog.Count.ToString() + " elemenata.");

    }
  }
}
