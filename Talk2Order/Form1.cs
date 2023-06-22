using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Windows.Forms;

namespace Talk2Order
{
    public partial class Form1 : Form
    {
        private SpeechRecognitionEngine recognizer;
        private Dictionary<string, decimal> itemCosts;


        public Form1()
        {
            InitializeComponent();
            InitializeSpeechRecognition();

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void InitializeSpeechRecognition()
        {
            recognizer = new SpeechRecognitionEngine();
            recognizer.SetInputToDefaultAudioDevice();
            recognizer.SpeechRecognized += Recognizer_SpeechRecognized;

            itemCosts = GetItemCostsFromTextFile();

            GrammarBuilder grammarBuilder = new GrammarBuilder();
            grammarBuilder.Append(new Choices(itemCosts.Keys.ToArray()));

            Grammar grammar = new Grammar(grammarBuilder);
            recognizer.LoadGrammar(grammar);

            recognizer.RecognizeAsync(RecognizeMode.Multiple);
        }

        private Dictionary<string, decimal> GetItemCostsFromTextFile()
        {
            string filePath = @"C:\test\items.txt";
            string[] lines = File.ReadAllLines(filePath);

            Dictionary<string, decimal> itemCosts = new Dictionary<string, decimal>();

            foreach (string line in lines)
            {
                string[] parts = line.Split(' ');
                string itemName = parts[0];
                decimal itemCost = decimal.Parse(parts[1]);
                itemCosts[itemName.ToLower()] = itemCost;
            }

            return itemCosts;
        }

        private void Recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string recognizedText = e.Result.Text;
            recognizedText = recognizedText.ToLower();

            int itemOccurrences = itemCosts.Keys.Count(item => item.ToLower().Contains(recognizedText));
            if (itemOccurrences > 0)
            {
                MessageBox.Show($"Found {itemOccurrences} item(s) containing '{recognizedText}':\n\n{GetMatchingItemsAsString(recognizedText)}");
            }
            else
            {
                MessageBox.Show("Item not found.");
            }
        }

        private string GetMatchingItemsAsString(string recognizedText)
        {
            IEnumerable<string> matchingItems = itemCosts.Keys.Where(item => item.ToLower().Contains(recognizedText));
            return string.Join("\n", matchingItems);
        }
    }
}