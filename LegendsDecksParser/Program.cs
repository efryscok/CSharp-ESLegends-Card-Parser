using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LegendsDecksParser {
    class LegendsCard {
        private static string tab = "\t";

        public string name;
        public string rarity;
        public string type;
        public string attributes;
        public string race;
        public string magickaCost;
        public string expansionSet;
        public string soulSummon;
        public string soulTrap;
        public string text;
        public string keywords;
        public string unlockedIn;
        public string attack;
        public string health;
        public string evolvesFrom;

        public string toStringTabDelimited() {
            return this.name + tab +
                   this.rarity + tab +
                   this.type + tab +
                   this.attributes + tab +
                   this.race + tab +
                   this.magickaCost + tab +
                   this.expansionSet + tab +
                   this.soulSummon + tab +
                   this.soulTrap + tab +
                   this.text + tab +
                   this.keywords + tab +
                   this.unlockedIn + tab +
                   this.attack + tab +
                   this.health + tab +
                   this.evolvesFrom;
        }
    }

    class Program {
       static Regex alphanumericWithPunctuation = new Regex("[^a-zA-Z0-9 -/.+,:]");

        public static string capitalizeWord(string word) {
            return word.First().ToString().ToUpper() + word.Substring(1);
        }

        public static string extractText(string text) {
            return alphanumericWithPunctuation.Replace(text, "");
        }

        static void Main(string[] args) {
            Console.Write("Please enter the folder's full path: ");

            string legendsDecksFilename = Console.ReadLine() + "\\LegendsDecks.txt";
            string cardListUrl = "https://www.legends-decks.com/cards";
            List<string> cardInfoUrls = new List<string>();
            System.IO.StreamWriter tabDelimtedFile = new System.IO.StreamWriter(legendsDecksFilename);

            Console.Write("\nLoading card list.");

            while (cardListUrl != null) {
                Console.Write(".");

                HtmlWeb page = new HtmlWeb();
                HtmlDocument doc = page.Load(cardListUrl);

                HtmlNode cardList = doc.DocumentNode.SelectNodes("/html[1]/body[1]/div[4]/div[1]/div[1]/div[1]/div[1]/div[2]/div[1]/div[5]")[0];

                foreach (HtmlNode item in cardList.ChildNodes) {
                    if (item.Name == "div" && item.HasChildNodes && item.ChildNodes[1].Name == "a") {
                        cardInfoUrls.Add(item.ChildNodes[1].Attributes[0].Value);
                    }
                }

                HtmlNode pageNavigation = doc.DocumentNode.SelectNodes("/html[1]/body[1]/div[4]/div[1]/div[1]/div[1]/div[1]/div[2]/div[3]")[0];

                foreach (HtmlNode item in pageNavigation.ChildNodes) {
                    if (item.InnerText == "&gt;") {
                        cardListUrl = item.Attributes[1].Value;
                        break;
                    }
                    else {
                        cardListUrl = null;
                    }
                }
            }

            Console.Write("\n\nLoading card info.");

            int index = 0;
            foreach (string url in cardInfoUrls) {
                if (++index % 42 == 0) {
                    Console.Write(".");
                }

                HtmlWeb page = new HtmlWeb();
                HtmlDocument doc = page.Load(url);

                HtmlNode table = doc.DocumentNode.SelectNodes("/html[1]/body[1]/div[4]/div[1]/div[1]/div[1]/div[1]/div[2]/div[1]/div[2]/div[1]/table[1]/tbody[1]")[0];

                LegendsCard card = new LegendsCard();

                foreach (HtmlNode row in table.ChildNodes) {
                    if (row.Name == "tr") {
                        string leftColumn = extractText(row.ChildNodes[1].InnerText);
                        string rightColumn = extractText(row.ChildNodes[3].InnerText);

                        switch (leftColumn) {
                            case "Name":
                                card.name = rightColumn;
                                break;
                            case "Rarity":
                                card.rarity = rightColumn;
                                break;
                            case "Type":
                                card.type = rightColumn;
                                break;
                            case "Attributes":
                                card.attributes = capitalizeWord(extractText(row.ChildNodes[3].ChildNodes[1].Attributes[1].Value));
                                break;
                            case "Race":
                                card.race = rightColumn;
                                break;
                            case "Magicka Cost":
                                card.magickaCost = rightColumn;
                                break;
                            case "Expansion set":
                                card.expansionSet = rightColumn;
                                break;
                            case "Soul Summon":
                                card.soulSummon = rightColumn;
                                break;
                            case "Soul Trap":
                                card.soulTrap = rightColumn;
                                break;
                            case "Text":
                                card.text = rightColumn;
                                break;
                            case "Keywords":
                                card.unlockedIn = rightColumn;
                                break;
                            case "Unlocked In":
                                card.keywords = rightColumn;
                                break;
                            case "Attack":
                                card.attack = rightColumn;
                                card.attack = card.attack.Substring(0, card.attack.Length - 5);
                                break;
                            case "Health":
                                card.health = rightColumn;
                                card.health = card.health.Substring(0, card.health.Length - 5);
                                break;
                            case "Evolves From":
                                card.evolvesFrom = rightColumn;
                                break;
                        }
                    }
                }

                tabDelimtedFile.WriteLine(card.toStringTabDelimited());
            }

            tabDelimtedFile.Close();

            Console.WriteLine("\n\nSaved in " + legendsDecksFilename);
            Console.Write("\nPress any key to continue . . . ");
            Console.ReadKey();
        }
    }
}
