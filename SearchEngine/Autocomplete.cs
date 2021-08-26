using System;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;

namespace SearchEngine{
    //This is the trie class for a creating a trie tree
    public class trie{
        public node root;

        //The class in initialized by creating a root node which is usually an empty string.
        public trie(){
            this.root = new node('\0');
        }

        //The insert function for inserting words into the trie tree
        //it takes a string and loops through each character in the string.
        //we start by initializing a current node which is first the root node
        //and we go down the tree by checking the children of the current node
        //if the children is null we add the character there, if it is not then we move to the next one.
        //and we repeat this till the loop is complete
        public void insert (string word){
            node curr = root;
            for (int i = 0; i < word.Length; i++){
                char c = word[i];
                node temp = new node(c);
                if(curr.children[c -'a'] == null){
                    curr.children[c - 'a'] = temp;
                }
                curr = curr.children[c - 'a'];
            }
            curr.isWord = true;
        }


        //this is to check prefixes and the main part of the autocomplete
        //it takes a string which is going to be the word being typed
        //and check if the word is in the trie or if we can move from the begining to the end in the tree
        //it uses an helper function the getNode function
        public node startWith(string word){
            if(getNode(word) != null){
                var node = getNode(word);
                return node;
            }
            else{
                return null;
            }
        }

        //this is the get node function it checks if a word is in the trie tree
        public node getNode (string word){
            node curr = root;
            for (int i = 0; i < word.Length; i++){
                char c = word[i];
                if(curr.children[c - 'a'] == null){
                    return null;
                }
                curr = curr.children[c - 'a'];
            }

            return curr;
        }

        //If a prefix is in the trie tree this functions helps get the words that has that prefix
        //the isWord method basically checks if the node we are passing is a leaf node
        //if it is then we add it to the List
        //We use 26 in the loop here because each node in the trie can have up to 26 children
        public void addAllwords(node node, string word, List<string> words){

            if(node.isWord == true){
                words.Add(word);
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



        //this is the class for creating nodes for the trie tree
        public class node{
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


    //The autocomplete class
    public class Autocomplete{
        public List<string> words;
        public List<string> result;
        public trie WordTree;

        //It is initialized by passing a List of strings to the constructor
        //A trie named WordTree is created on initialization
        //words from string List are also added to the WordTree at initialization
        public Autocomplete(List<string> args){
            this.words = args;
            this.WordTree = new trie();
            for (int i = 0; i < this.words.Count; i++) {
                this.WordTree.insert(this.words[i]);
            }
        }


        //this is the main function
        //it takes a string which might be a prefix or a word
        //using the trie methods created above it generates a list of words
        //the list of words is converting into a string array which is the return value
        public string[] auto(string args){
            try{
                if(this.WordTree.startWith(args) != null){
                    var node = WordTree.startWith(args);
                    this.WordTree.addAllwords(node, args, result);

                    string[] myWords = result.ToArray();
                    return myWords;
                }
                else{
                    return new[] {"error", "error"};
                }
            }
            catch{
                return new[] { "error", "error" };
            }
        }
    }
}


