using System;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using AngleSharp;

namespace SearchEngine{
    
    public class trie{
        private node root;

        public trie(){
            this.root = new node('\0');
        }

        public void insert (String word){
            node curr = root;
            for (int i = 0; i < word.length(); i++){
                char c = word.charAt(i);
                if(curr.children[c -'a'] == null){
                    curr.children[c - 'a'] = c;
                }
                curr = curr.children[c - 'a'];
            }
            curr.isWord = true;
        }

        public node startWith(string word){
            if(getNode(word) != null){
                var node = getNode(word);
                return node;
            }
            else{
                return null;
            }
        }

        private node getNode (string word){
            node curr = root;
            for (int i = 0; i < word.length(); i++){
                char c = word.charAt(i);
                if(curr.children[c - 'a'] == null){
                    return null;
                }
                curr = curr.children[c - 'a'];
            }

            return curr;
        }

        public void addAllwords(node node, String word, List<string> words){

            if(node.isWord == true){
                words.add(word);
            }

            for (int i = 0; i < 26; i++)
            {
                node next = node.children[((char)(i + 'a')) - 'a'];
                if (next != null)
                {
                    addAllwords(next, word + (char)(i + 'a'), words);
                }
            }

        }

        class node{
            public char c;
            public bool isWord;
            public node[] children;


            public node(char c){
                this.c = c;
                this.isWord = false;
                this.children = new node[26];
            }

        }
    }

    public class Autocomplete{
        public List<T> words;

        public trie WordTree;

        public Autocomplete(List<T> args){
            this.words = args;
            this.WordTree = new trie();
        }

        public string[] auto(string args){
            List<string> result;
            for (int i = 0; i < this.words.Count; i++) {
                this.WordTree.insert(this.words[i]);
            }
            try{
            if(this.WordTree.startWith(args) != null){
                    var node = WordTree.startWith(args);
                    this.WordTree.addAllwords(node, args, result);

                    string[] myWords = result.ToArray();
                    return myWords;
                }
            }
            catch{
                return new[] { "error", "error" };
            }
        }
    }
}


