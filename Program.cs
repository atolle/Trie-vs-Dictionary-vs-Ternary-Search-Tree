using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

public class TrieNode
{
    public char? Value { get; set; }
    public Dictionary<char, TrieNode> Children { get; private set; }
    public bool IsEndOfWord { get; set; }

    public TrieNode(char? value)
    {
        Value = value;
        Children = new Dictionary<char, TrieNode>();
        IsEndOfWord = false;
    }
}

public class TernarySearchTree
{
    private class Node
    {
        public char Char { get; set; }
        public bool IsEnd { get; set; }
        public Node Left { get; set; }
        public Node Middle { get; set; }
        public Node Right { get; set; }
    }

    private Node root;

    public void Insert(string key)
    {
        root = Insert(root, key, 0);
    }

    private Node Insert(Node node, string key, int depth)
    {
        char c = key[depth];

        if (node == null)
        {
            node = new Node { Char = c };
        }

        if (c < node.Char)
        {
            node.Left = Insert(node.Left, key, depth);
        }
        else if (c > node.Char)
        {
            node.Right = Insert(node.Right, key, depth);
        }
        else if (depth < key.Length - 1)
        {
            node.Middle = Insert(node.Middle, key, depth + 1);
        }
        else
        {
            node.IsEnd = true;
        }

        return node;
    }

    public List<string> RetrieveByWildcard(string wildcard)
    {
        var results = new List<string>();
        RetrieveByWildcard(root, wildcard, 0, "", results);
        return results;
    }

    private void RetrieveByWildcard(Node node, string key, int depth, string prefix, List<string> results)
    {
        if (node == null)
            return;

        char c = key[depth];
        var nextCharIsWildcard = false;

        if (depth < key.Length - 1)
        {
            nextCharIsWildcard = key[depth + 1] == '*';
        }        
        
        if (c == '*' || c < node.Char)
        {
            RetrieveByWildcard(node.Left, key, depth, prefix, results);           
        }            
        
        if (c == '*' || c == node.Char)
        {
            if ((depth == key.Length - 1 || c == '*' || nextCharIsWildcard) && node.IsEnd)
            {
                results.Add(prefix + node.Char);
            }

            if (depth < key.Length - 1)
            {
                RetrieveByWildcard(node.Middle, key, depth + 1, prefix + node.Char, results);
            }             
            else if (c == '*')
            {
                RetrieveByWildcard(node.Middle, key, depth, prefix + node.Char, results);
            }
        }
        
        if (c == '*' || c > node.Char)
        {
            RetrieveByWildcard(node.Right, key, depth, prefix, results);
        }            
    }
}

public class Trie
{
    private TrieNode root;

    public Trie()
    {
        root = new TrieNode(null);
    }

    public void Insert(string word)
    {
        TrieNode node = root;
        foreach (char c in word)
        {
            if (!node.Children.ContainsKey(c))
            {
                node.Children[c] = new TrieNode(c);
            }
            node = node.Children[c];
        }
        node.IsEndOfWord = true;
    }

    public List<string> WildcardSearch(string pattern)
    {
        List<string> result = new List<string>();
        WildcardSearchUtil(root, pattern, "", result);
        return result;
    }

    private void WildcardSearchUtil(TrieNode node, string pattern, string prefix, List<string> result)
    {
        if (string.IsNullOrEmpty(pattern))
        {
            if (node.IsEndOfWord)
            {
                result.Add(prefix + node.Value);
            }
            foreach (var child in node.Children.Values)
            {
                WildcardSearchUtil(child, pattern, prefix + node.Value, result);
            }
        }
        else
        {
            char c = pattern[0];
            if (c == '*')
            {
                foreach (var child in node.Children.Values)
                {
                    WildcardSearchUtil(child, pattern.Substring(1), prefix, result);
                }
            }
            else if (node.Children.ContainsKey(c))
            {
                WildcardSearchUtil(node.Children[c], pattern.Substring(1), prefix + c, result);
            }
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        Trie trie = new Trie();
        TernarySearchTree tst = new TernarySearchTree();
        ConcurrentDictionary<string, string> dictionary = new ConcurrentDictionary<string, string>();
        List<string> keys = new List<string>();
        List<string> prefixes = new List<string>() { "google.com" + ":" + Guid.NewGuid(), "yahoo.com" + ":" + Guid.NewGuid(), "something.com" + ":" + Guid.NewGuid(), "whatever.com" + ":" + Guid.NewGuid(), "cnn.com" + ":" + Guid.NewGuid(), "youtube.com" + ":" + Guid.NewGuid(), "chatgpt.com" + ":" + Guid.NewGuid(), "akumina.com" + ":" + Guid.NewGuid(), "test.com" + ":" + Guid.NewGuid(), "somethingelse.com" + ":" + Guid.NewGuid() };

        for (int i = 0; i < 2000000; i++)
        {
            Random random = new Random();
            int randomNumber = random.Next(0, prefixes.Count);
            var key = prefixes[randomNumber] + ":" + Guid.NewGuid() + ":" + Guid.NewGuid() + ":" + Guid.NewGuid();

            keys.Add(key);

            Console.WriteLine(key);
        }

        //string[] keys = { "dog 1", "dog 1", "dog 2", "dog 3", "cat 4", "cat 5", "dog12789758917", "dotg9895", "do g", "dooog", "dog    432" };

        long dictionaryMsTotal = 0;
        long tstMsTotal = 0;
        long longestDictionary = 0;
        long longestTst = 0;


        for (int i = 0; i < keys.Count; i++)
        {
            if (i % 10000 == 0)
            {
                Console.WriteLine("Progress: " + i);
            }
            
            //Stopwatch swTrie = new Stopwatch();
            Stopwatch swDictionary = new Stopwatch();
            Stopwatch swTst = new Stopwatch();

            //swTrie.Start();
            //Console.WriteLine("Insert into trie index: " + i + " | key: " + keys[i]);
            //trie.Insert(keys[i]);

            swTst.Start();

            //Console.WriteLine("Insert into tst index: " + i + " | key: " + keys[i]);
            tst.Insert(keys[i]);

            swTst.Stop();

            if (swTst.ElapsedMilliseconds > longestTst) longestTst = swTst.ElapsedMilliseconds;

            tstMsTotal += swTst.ElapsedMilliseconds;

            //swTrie.Stop();
            swDictionary.Start();

            //Console.WriteLine("Insert into dictionary index: " + i + " | key: " + keys[i]);
            dictionary.TryAdd(keys[i], "");

            swDictionary.Stop();

            if (swDictionary.ElapsedMilliseconds > longestDictionary) longestDictionary = swDictionary.ElapsedMilliseconds;

            dictionaryMsTotal += swDictionary.ElapsedMilliseconds;

            //Console.WriteLine($"Trie write time: {swTrie.ElapsedMilliseconds} ms | Dictionary write time: {swDictionary.ElapsedMilliseconds}");            
        }

        Console.WriteLine($"Tst avg insert time - longest: {(float)tstMsTotal / keys.Count} ms - {longestTst} ms | Dictionary avg insert time: {(float)dictionaryMsTotal / keys.Count} - {longestDictionary} ms");

        while (true)
        {
            Console.WriteLine("Enter key to search: ");
            var input = Console.ReadLine();

            // Wildcard search


            Stopwatch swTrie = new Stopwatch();
            Stopwatch swDictionary = new Stopwatch();
            Stopwatch swTst = new Stopwatch();
            //swTrie.Start();

            //var trieResults = trie.WildcardSearch(input ?? "");

            //swTrie.Stop();

            swDictionary.Start();

            var dictionaryResults = DictionarySearch(dictionary, input ?? "");

            swDictionary.Stop();

            swTst.Start();

            var tstResults = tst.RetrieveByWildcard(input ?? "");

            swTst.Stop();

            //Console.WriteLine($"Trie result count: {trieResults.Count} ms | Tst result count: {tstResults.Count} | Dictionary result count: {dictionaryResults.Count}");

            Console.WriteLine($"Tst result count: {tstResults.Count} | Dictionary result count: {dictionaryResults.Count}");

            //Console.WriteLine($"Trie search time: {swTrie.ElapsedMilliseconds} ms | Tst search time: {swTst.ElapsedMilliseconds} ms | Dictionary search time: {swDictionary.ElapsedMilliseconds} ms");

            Console.WriteLine($"Tst search time: {swTst.ElapsedMilliseconds} ms | Dictionary search time: {swDictionary.ElapsedMilliseconds} ms");
        }
    }

    static private List<string> DictionarySearch(ConcurrentDictionary<string, string> dictionary, string key)
    {
        var list = new List<string>();

        if (key.EndsWith("*"))
        {
            key = key.Remove(key.Length - 1);
        }

        foreach (var dictionaryKey in dictionary.Keys)
        {
            if (dictionaryKey.Contains(key))
            {
                list.Add(dictionaryKey);
            }
        }

        return list;
    }
}
