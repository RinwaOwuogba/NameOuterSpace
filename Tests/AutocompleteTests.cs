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
        public void TestAutoComplete()
        {
            List<string> words = new List<string>();

            words.Add("Bye");
            words.Add("Good");
            words.Add("Hello");
            words.Add("Goodbye");
            words.Add("sucks");

            Autocomplete autocomplete = new Autocomplete(words);

            var result = autocomplete.auto("by");
            Console.WriteLine(result[0]);
           CollectionAssert.AreEqual(new[] {"bye"}, result);

        }
    }

}