using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

class Program
{
    static void Main()
    {
        // Load scriptures from a file for extended functionality
        List<Scripture> scriptures = LoadScripturesFromFile("scriptures.txt");
        Random random = new Random();
        Scripture scripture = scriptures[random.Next(scriptures.Count)];

        while (!scripture.IsFullyHidden())
        {
            Console.Clear();
            Console.WriteLine(scripture.GetDisplayText());
            Console.WriteLine("\nPress Enter to hide words or type 'quit' to exit.");

            string input = Console.ReadLine();
            if (input.ToLower() == "quit") break;

            scripture.HideWords(3);
        }

        Console.Clear();
        Console.WriteLine(scripture.GetDisplayText());
        Console.WriteLine("\nMemorization complete!");
    }

    static List<Scripture> LoadScripturesFromFile(string filePath)
    {
        List<Scripture> scriptures = new List<Scripture>();
        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                string[] parts = line.Split('|');
                if (parts.Length == 2)
                {
                    scriptures.Add(new Scripture(new Reference(parts[0]), parts[1]));
                }
            }
        }
        return scriptures.Count > 0 ? scriptures : new List<Scripture> { new Scripture(new Reference("Proverbs 3:5-6"), "Trust in the Lord with all your heart and lean not on your own understanding; in all your ways submit to him, and he will make your paths straight.") };
    }
}

class Reference
{
    public string Book { get; }
    public int Chapter { get; }
    public int StartVerse { get; }
    public int? EndVerse { get; }

    public Reference(string reference)
    {
        var parts = reference.Split(new[] { ' ', ':' }, StringSplitOptions.RemoveEmptyEntries);
        Book = parts[0];
        Chapter = int.Parse(parts[1]);
        StartVerse = int.Parse(parts[2]);
        EndVerse = parts.Length > 3 ? int.Parse(parts[3]) : (int?)null;
    }

    public override string ToString()
    {
        return EndVerse == null ? $"{Book} {Chapter}:{StartVerse}" : $"{Book} {Chapter}:{StartVerse}-{EndVerse}";
    }
}

class Scripture
{
    private Reference Reference { get; }
    private List<Word> Words { get; }
    private static Random random = new Random();

    public Scripture(Reference reference, string text)
    {
        Reference = reference;
        Words = text.Split(' ').Select(word => new Word(word)).ToList();
    }

    public string GetDisplayText()
    {
        return $"{Reference}\n" + string.Join(" ", Words.Select(word => word.GetDisplayText()));
    }

    public void HideWords(int count)
    {
        List<Word> visibleWords = Words.Where(word => !word.IsHidden).ToList();
        int wordsToHide = Math.Min(count, visibleWords.Count);
        for (int i = 0; i < wordsToHide; i++)
        {
            int index = random.Next(visibleWords.Count);
            visibleWords[index].Hide();
            visibleWords.RemoveAt(index);
        }
    }

    public bool IsFullyHidden()
    {
        return Words.All(word => word.IsHidden);
    }
}

class Word
{
    private string Text { get; }
    public bool IsHidden { get; private set; }

    public Word(string text)
    {
        Text = text;
        IsHidden = false;
    }

    public void Hide()
    {
        IsHidden = true;
    }

    public string GetDisplayText()
    {
        return IsHidden ? new string('_', Text.Length) : Text;
    }
}
