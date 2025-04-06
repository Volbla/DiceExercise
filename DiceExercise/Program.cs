using static System.Console;

namespace DiceExercise {
    internal class Program {
        static void Main(string[] args) {
            WriteLine("Enter a series of dice to calculate the probability of each outcome of sums.");
            WriteLine("A single value counts as a single die with that many sides.");
            WriteLine("Multiple dice can be entered using ttrpg notation (e.g. 4d6 is four six-sided dice).");

            List<int> diceList = new List<int>();
            List<long> outcomeCounts = new List<long>();
            string input;
            long totalRolls;

            while (true) {
                //Write(">>"); input = ReadLine() ?? "";
                input = "3d6 3d8";
				if (input.Length == 0)
                    break;

                diceList.Clear();
                outcomeCounts.Clear();
                totalRolls = 1;
                ParseInput(input, diceList);

                int min = diceList.Count;
                int max = diceList.Sum();
                CountOutcomes(diceList, outcomeCounts, min, max);
                foreach (int die in diceList)
                    totalRolls *= die;

                long maxCount = outcomeCounts.Max();
                int pad = max.ToString().Length;
                for (int i = 0; i < outcomeCounts.Count; i++) {
                    Write((min + i).ToString().PadLeft(pad) + ": ");
                    //WriteLine(outcomeCounts[i]);
                    int size = (int)(40 * outcomeCounts[i] / maxCount);
                    WriteLine(new string('#', size) + $"  {outcomeCounts[i]}");
                }

                ReadKey(); break;
            }
        }

        static void ParseInput(string input, List<int> diceList) {
            foreach (string entry in input.Split([',', ' '], StringSplitOptions.RemoveEmptyEntries)) {
                if (entry.All(char.IsDigit)) {
                    diceList.Add(int.Parse(entry));
                } else {
                    string[] nums = entry.ToLower().Split('d');
                    var (count, sides) = (int.Parse(nums[0]), int.Parse(nums[1]));
                    for (int i = 0; i < count; i++) {
                        diceList.Add(sides);
                    }
                }
            }
        }

        static void CountOutcomes(List<int> diceList, List<long> outcomeCounts, int min, int max) {
            int sign, dCount = diceList.Count;

            List<List<int>> sideTots = new List<List<int>>();

            for (int dice = 0; dice < dCount; dice++) {
                sideTots.Add(new List<int>());

                if (dice == 0) {
                    sideTots[0].Add(0);
                    continue;
                }

                for (
					int[] currentComb = Enumerable.Range(0, dice).ToArray(); 
					currentComb[0] <= dCount - dice; 
					nextComb(currentComb, dCount, dice)
				) {
                    sideTots[dice].Add(0);
                    foreach (int j in currentComb)
						sideTots[dice][^1] += diceList[j];
				}

				Write(dice.ToString() + ": ");
				WriteLine(String.Join(' ', sideTots[dice]));
			}
			WriteLine();

			for (int x = min; x <= max; x++) {
                outcomeCounts.Add(0);

                for (int dice = 0; dice < dCount; dice++) {
                    sign = dice % 2 == 0 ? 1 : -1;

					foreach (int st in sideTots[dice].FindAll(st => st < x)) {
                        outcomeCounts[^1] += sign * Simplex(x - st, dCount);
					}
                }
            }
        }


        // General math functions.

        static long Combinations(int n, int r) {
            if (r > n || r < 0) return 0;
            if (r == n || r == 0) return 1;

            long c = 1;
            for (int i = 0; i < r; i++) {
                c *= n - i;
                c /= i + 1;
            }
            return c;
        }

        static long Simplex(int x, int d) {
            return Combinations(Math.Max(0, x - 1), d - 1);
        }

        static void nextComb(int[] current, int n, int r) {
            int i, j;

            // Iterate backwards through the list, carrying any overflow
            for (i = 1; i <= r; i++) {
                current[^i]++;
                if (current[^i] <= n - i) {
                    break;
                }
            }
            i = Math.Min(r, i);
            // Reset any overflown entries to their smallest valid value
            for (j = r - i + 1; j < r; j++) {
                current[j] = current[j - 1] + 1;
            }
        }
    }
}
