using System;
using System.Linq;

namespace Forberedelse_til_tirsdag_27_02
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Velkommen til spillet!");

            while (true) 
            {
                int numberOfPlayers;

                //spiller antal
                do
                {
                    Console.Write("Skriv antal spillere (Minimum 2, maksimum 5): ");
                } while (!int.TryParse(Console.ReadLine(), out numberOfPlayers) || numberOfPlayers < 2 || numberOfPlayers > 5);

                Console.WriteLine("Du har valgt " + numberOfPlayers + " spillere.");

                //Array til at gemme spillernavne
                string[] playerNames = new string[numberOfPlayers];

                //Array til at gemme scores for hver spiller og yatzykombinationer
                int[,] scores = new int[numberOfPlayers, 14]; 

                //Array til at gemme resultaterne af terningkast for hver spiller
                int[][] allDiceResults = new int[numberOfPlayers][];

                // Array til at gemme de valgte kombinationer for hver spiller
                int[] chosenCombinations = new int[numberOfPlayers];

                //loop der beder hver spiller om deres navn, og gemmer i array
                for (int i = 0; i < numberOfPlayers; i++)
                {
                    Console.Write($"Indtast navn for spiller {i + 1}: ");
                    playerNames[i] = Console.ReadLine();
                }

                Console.WriteLine("\nSpillernavne:");
                //Loop der udskriver navnene på spillerne
                for (int i = 0; i < numberOfPlayers; i++)
                {
                    Console.WriteLine($"Spiller {i + 1}: {playerNames[i]}");
                }

                // Loop der kører spillet, indtil alle kombinationer er fyldt ud
                while (true)
                {
                    //loop der lader hver spiller udføre terningkast
                    for (int playerIndex = 0; playerIndex < numberOfPlayers; playerIndex++)
                    {
                        Console.WriteLine($"\n{playerNames[playerIndex]}, tryk på en vilkårlig tast for at kaste terningerne! ");
                        Console.ReadKey();
                        int[] diceResults = RollDice(5);
                        Console.WriteLine($"\nResultater af kast for {playerNames[playerIndex]}: ");
                        foreach (var result in diceResults)
                        {
                            Console.Write(result + " ");
                        }
                        // Loop til Ekstra kast
                        for (int kast = 1; kast <= 2; kast++)
                        {
                            Console.WriteLine($"\n{kast}. vil du kaste terninger igen? (Ja/Nej)");
                            string response = Console.ReadLine();
                            if (response.ToLower() == "ja")
                            {
                                Console.WriteLine("Hvilke terninger vil du kaste igen? skriv deres positioner adskildt af space: ");
                                string[] positions = Console.ReadLine().Split(' ');

                                //Loop der kaster valgte terninger
                                foreach (var pos in positions)
                                {
                                    int position = int.Parse(pos);
                                    diceResults[position - 1] = RollSingleDie();
                                }
                                Console.WriteLine($"Resultater af {kast}. kast for {playerNames[playerIndex]}: ");
                                foreach (var result in diceResults)
                                {
                                    Console.Write(result + " ");
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        // Lad spilleren vælge yatzy-kombination
                        Console.WriteLine($"\nVælg en yatzy-kombination (1-14) for {playerNames[playerIndex]}: ");
                        Console.WriteLine("1. 1'ere");
                        Console.WriteLine("2. 2'ere");
                        Console.WriteLine("3. 3'ere");
                        Console.WriteLine("4. 4'ere");
                        Console.WriteLine("5. 5'ere");
                        Console.WriteLine("6. 6'ere");
                        Console.WriteLine("7. Ét par");
                        Console.WriteLine("8. To par");
                        Console.WriteLine("9. Tre ens");
                        Console.WriteLine("10. Fire ens");
                        Console.WriteLine("11. Lille straight");
                        Console.WriteLine("12. Store Straight");
                        Console.WriteLine("13. Chancen");
                        Console.WriteLine("14. Yatzy");
                        int combinationChoice;
                        while (!int.TryParse(Console.ReadLine(), out combinationChoice) || combinationChoice < 1 || combinationChoice > 14)
                        {
                            Console.WriteLine("Ugyldigt valg. Vælg en yatzy-kombination (1-14): ");
                        }
                        chosenCombinations[playerIndex] = combinationChoice;
                        allDiceResults[playerIndex] = diceResults;
                    }

                    // Opdater scores baseret på de valgte kombinationer
                    for (int playerIndex = 0; playerIndex < numberOfPlayers; playerIndex++)
                    {
                        int chosenCombination = chosenCombinations[playerIndex];
                        int[] diceResults = allDiceResults[playerIndex];
                        // Hvis kombinationen allerede er valgt, skal vi springe over
                        if (scores[playerIndex, chosenCombination - 1] == 0)
                        {
                            scores[playerIndex, chosenCombination - 1] = GetCombinationScore(chosenCombination, diceResults);
                        }
                    }

                    // Tjek om alle kombinationer er fyldt ud
                    bool allCombinationsFilled = true;
                    for (int playerIndex = 0; playerIndex < numberOfPlayers; playerIndex++)
                    {
                        for (int combinationIndex = 0; combinationIndex < 14; combinationIndex++) // Ændret til 14
                        {
                            if (scores[playerIndex, combinationIndex] == 0)
                            {
                                allCombinationsFilled = false;
                                break;
                            }
                        }
                    }

                    // Hvis alle kombinationer er fyldt ud, afslut spillet
                    if (allCombinationsFilled)
                    {
                        break;
                    }

                    // Udskriv scoreboardet efter hver spillers tur
                    Console.WriteLine("\nScoreboard:");
                    for (int i = 0; i < numberOfPlayers; i++)
                    {
                        Console.WriteLine($"Spiller {playerNames[i]}:");
                        for (int j = 0; j < 13; j++) // Dette loop går kun til 13
                        {
                            string combinationName = GetCombinationName(j + 1, allDiceResults[i]);
                            int combinationScore = scores[i, j];
                            Console.WriteLine($"{j + 1}. {combinationName} = {combinationScore} point");
                        }

                        // Tilføj Yatzy-kombinationen separat
                        Console.WriteLine($"14. {GetCombinationName(14, allDiceResults[i])} = {scores[i, 13]} point");

                        // Beregn og vis total score for hver spiller
                        int totalScore = 0;
                        for (int j = 0; j < 13; j++) // Dette loop går kun til 13
                        {
                            totalScore += scores[i, j];
                        }
                        Console.WriteLine("Total Score: " + totalScore);
                    }
                }

                // Vis det endelige scoreboard
                Console.WriteLine("\nEndeligt Scoreboard:");
                for (int i = 0; i < numberOfPlayers; i++)
                {
                    Console.WriteLine($"Spiller {playerNames[i]}:");
                    for (int j = 0; j < 14; j++)
                    {
                        string combinationName = GetCombinationName(j + 1, allDiceResults[i]);
                        int combinationScore = scores[i, j];
                        Console.WriteLine($"{j + 1}. {combinationName} = {combinationScore} point");
                    }

                    // Tilføj Yatzy-kombinationen separat
                    Console.WriteLine($"14. {GetCombinationName(14, allDiceResults[i])} = {scores[i, 13]} point");

                    // Beregn og vis total score for hver spiller
                    int totalScore = 0;
                    for (int j = 0; j < 14; j++)
                    {
                        totalScore += scores[i, j];
                    }
                    Console.WriteLine("Total Score: " + totalScore);
                }

                // Spørg om spillet skal startes forfra
                Console.WriteLine("\nVil du starte et nyt spil? (Ja/Nej)");
                string restartResponse = Console.ReadLine();
                if (restartResponse.ToLower() != "ja")
                {
                    break;
                }
            }
        }

        // Metode til at slå en enkelt terning
        static int RollSingleDie()
        {
            Random rnd = new Random();
            return rnd.Next(1, 7);
        }

        // Metode til at slå flere terninger
        static int[] RollDice(int numberOfDice)
        {
            int[] results = new int[numberOfDice];
            for (int i = 0; i < numberOfDice; i++)
            {
                results[i] = RollSingleDie();
            }
            return results;
        }

        // Metode til at tælle antallet af terninger med en bestemt værdi
        static int CountDiceWithValue(int[] diceResults, int value)
        {
            int count = 0;
            foreach (int result in diceResults)
            {
                if (result == value)
                {
                    count++;
                }
            }
            return count;
        }

        // Metode til at beregne point for en yatzy-kombination
        static int GetCombinationScore(int combination, int[] diceResults)
        {
            
            switch (combination)
            {
                case 1: // 1'ere
                    return CountDiceWithValue(diceResults, 1) * 1;
                case 2: // 2'ere
                    return CountDiceWithValue(diceResults, 2) * 2;
                case 3: // 3'ere
                    return CountDiceWithValue(diceResults, 3) * 3;
                case 4: // 4'ere
                    return CountDiceWithValue(diceResults, 4) * 4;
                case 5: // 5'ere
                    return CountDiceWithValue(diceResults, 5) * 5;
                case 6: // 6'ere
                    return CountDiceWithValue(diceResults, 6) * 6;
                case 7: // Ét par
                    return GetOnePairScore(diceResults);
                case 8: // To par
                    return GetTwoPairsScore(diceResults);
                case 9: // Tre ens
                    return GetThreeOfAKindScore(diceResults);
                case 10: // Fire ens
                    return GetFourOfAKindScore(diceResults);
                case 11: // Lille straight
                    return GetSmallStraightScore(diceResults);
                case 12: // Store Straight
                    return GetLargeStraightScore(diceResults);
                case 13: // Chancen
                    return diceResults.Sum();
                case 14: // Yatzy
                    return GetYatzyScore(diceResults);
                default:
                    return 0;
            }
        }

        // Metode til at få navnet på en yatzy-kombination baseret på dens nummer
        static string GetCombinationName(int combination, int[] diceResults)
        {
            switch (combination)
            {
                case 1: // 1'ere
                    return "1'ere";
                case 2: // 2'ere
                    return "2'ere";
                case 3: // 3'ere
                    return "3'ere";
                case 4: // 4'ere
                    return "4'ere";
                case 5: // 5'ere
                    return "5'ere";
                case 6: // 6'ere
                    return "6'ere";
                case 7: // Ét par
                    return "Ét par";
                case 8: // To par
                    return "To par";
                case 9: // Tre ens
                    return "Tre ens";
                case 10: // Fire ens
                    return "Fire ens";
                case 11: // Lille straight
                    return "Lille straight";
                case 12: // Store Straight
                    return "Store straight";
                case 13: // Chancen
                    return "Chancen";
                case 14: // Yatzy
                    return "Yatzy";
                default:
                    return "Ugyldig kombination";
            }
        }

        // Metode til at beregne point for ét par
        static int GetOnePairScore(int[] diceResults)
        {
            Array.Sort(diceResults);
            for (int i = diceResults.Length - 1; i > 0; i--)
            {
                if (diceResults[i] == diceResults[i - 1])
                {
                    return diceResults[i] * 2;
                }
            }
            return 0;
        }

        // Metode til at beregne point for to par
        static int GetTwoPairsScore(int[] diceResults)
        {
            Array.Sort(diceResults);
            int pairCount = 0;
            int score = 0;
            for (int i = diceResults.Length - 1; i > 0; i--)
            {
                if (diceResults[i] == diceResults[i - 1])
                {
                    score += diceResults[i] * 2;
                    pairCount++;
                    i--;
                }
                if (pairCount == 2)
                {
                    return score;
                }
            }
            return 0;
        }

        // Metode til at beregne point for tre ens
        static int GetThreeOfAKindScore(int[] diceResults)
        {
            Array.Sort(diceResults);
            for (int i = 0; i < diceResults.Length - 2; i++)
            {
                if (diceResults[i] == diceResults[i + 1] && diceResults[i] == diceResults[i + 2])
                {
                    return diceResults[i] * 3;
                }
            }
            return 0;
        }

        // Metode til at beregne point for fire ens
        static int GetFourOfAKindScore(int[] diceResults)
        {
            Array.Sort(diceResults);
            for (int i = 0; i < diceResults.Length - 3; i++)
            {
                if (diceResults[i] == diceResults[i + 1] && diceResults[i] == diceResults[i + 2] && diceResults[i] == diceResults[i + 3])
                {
                    return diceResults[i] * 4;
                }
            }
            return 0;
        }

        // Metode til at beregne point for lille straight
        static int GetSmallStraightScore(int[] diceResults)
        {
            Array.Sort(diceResults);
            for (int i = 0; i < diceResults.Length - 1; i++)
            {
                if (diceResults[i] != diceResults[i + 1] && diceResults[i + 1] - diceResults[i] != 1)
                {
                    return 0;
                }
            }
            return 15;
        }

        // Metode til at beregne point for stor straight
        static int GetLargeStraightScore(int[] diceResults)
        {
            Array.Sort(diceResults);
            for (int i = 0; i < diceResults.Length - 1; i++)
            {
                if (diceResults[i] != diceResults[i + 1] && diceResults[i + 1] - diceResults[i] != 1)
                {
                    return 0;
                }
            }
            return 20;
        }

        // Metode til at beregne point for yatzy
        static int GetYatzyScore(int[] diceResults)
        {
            for (int i = 0; i < diceResults.Length - 1; i++)
            {
                if (diceResults[i] != diceResults[i + 1])
                {
                    return 0;
                }
            }
            return 50;
        }
    }
}
