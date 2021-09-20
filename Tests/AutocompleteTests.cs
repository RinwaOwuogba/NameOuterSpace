using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SearchEngine;


namespace Tests
{

    [TestClass]
    public class AutocompleteTest
    {

        [TestMethod]
        public void TestAutocompleteInit()
        {
            List<string> words = new List<string>();

            words.Add("Bye");
            words.Add("Good");
            words.Add("Hello");
            words.Add("Goodbye");
            words.Add("sucks");

            Autocomplete autocomplete = new Autocomplete(words);

            Assert.AreEqual(words, autocomplete.getWords());
        }

        [TestMethod]
        public void TestAutocompleteInit2()
        {
            List<string> words = new List<string>();

            words.Add("Bye");
            words.Add("Good");
            words.Add("Hello");
            words.Add("Goodbye");
            words.Add("12");

            Autocomplete autocomplete = new Autocomplete(words);

            Assert.AreEqual(words, autocomplete.getWords());
        }

        [TestMethod]
        public void TestAutoComplete()
        {
            List<string> words = new List<string>();
            string[] result;

            words.Add("Bye");
            words.Add("Good");
            words.Add("Hello");
            words.Add("Goodbye this is the end");
            words.Add("sucks");
            words.Add("bye");
            words.Add("1987");

            Autocomplete autocomplete = new Autocomplete(words);
            string test = "Good";
            result = autocomplete.auto(test);

            CollectionAssert.AreEqual(new[] { "Good", "Goodbye this is the end" }, result);

        }


    }

}