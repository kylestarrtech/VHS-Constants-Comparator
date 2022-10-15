using System.IO;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// See https://aka.ms/new-console-template for more information
Console.WriteLine(" __      ___    _  _____    _____ ____  _   _  _____ _______       _   _ _______ _____ ");
Console.WriteLine(" \\ \\    / / |  | |/ ____|  / ____/ __ \\| \\ | |/ ____|__   __|/\\   | \\ | |__   __/ ____|");
Console.WriteLine("  \\ \\  / /| |__| | (___   | |   | |  | |  \\| | (___    | |  /  \\  |  \\| |  | | | (___  ");
Console.WriteLine("   \\ \\/ / |  __  |\\___ \\  | |   | |  | | . ` |\\___ \\   | | / /\\ \\ | . ` |  | |  \\___ \\ ");
Console.WriteLine("    \\  /  | |  | |____) | | |___| |__| | |\\  |____) |  | |/ ____ \\| |\\  |  | |  ____) |");
Console.WriteLine("     \\/   |_|  |_|_____/   \\_____\\____/|_| \\_|_____/   |_/_/    \\_\\_| \\_|  |_| |_____/ \n\n");

// Request user input for path
Console.Write("\n\nCurrent Constants Path: ");
string newPath = Console.ReadLine();

Console.Write("\nOld Constants Path: ");
string oldPath = Console.ReadLine();

Console.Write("\nOutput Path: ");
string output = Console.ReadLine();

if (!File.Exists(newPath) || !File.Exists(oldPath))
{
    Console.WriteLine("One or more of your input paths do not exist! Check path names!");
    return;
}

string oldConst = File.ReadAllText(oldPath);

string newConst = File.ReadAllText(newPath);

Dictionary<string, dynamic> oldValues = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(oldConst);
Dictionary<string, dynamic> newValues = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(newConst);

Dictionary<string, dynamic> uniqueKeys = new Dictionary<string, dynamic>();

List<string> outputChanges = new List<string>();

foreach (string key in newValues.Keys)
{
    /* Compare with same key in oldValues
     * If key does not exist in oldValues, add it to separate dictionary of uniquely new keys.
     * If value in oldValues is same, ignore. Otherwise, add to another dictionary of changed keys. */


    if (!oldValues.ContainsKey(key))
    {
        uniqueKeys.Add(key, newValues[key]);
        continue;
    }

    string valueComparison = "";

    Console.WriteLine($"{key}: {newValues[key].GetType().FullName}");

    if (newValues[key] is JObject)
    {
        Console.WriteLine("\tIS JOBJECT!");

        JObject newObj = newValues[key];
        JObject oldObj = oldValues[key];


        /*
         Compare new[key] to old[key]
            If difference, break out.
            Otherwise, continue.
         */
        foreach (var kvPair in newObj)
        {
            Console.WriteLine($"\t\t{kvPair.Key}: {oldObj[kvPair.Key]} vs {kvPair.Key}: {kvPair.Value}");

            if (!oldObj.ContainsKey(kvPair.Key)) {
                uniqueKeys.Add(kvPair.Key, kvPair.Value);
            }

            if (!JToken.DeepEquals(oldObj.GetValue(kvPair.Key), kvPair.Value))
            {
                Console.WriteLine($"\t\t\tDIFFERENCE FOUND IN THIS KEY!");
                outputChanges.Add($"{key}: {oldValues[key]} >> {newValues[key]}");
                break;
            }
        }

        continue;
    }

    if (newValues[key] == oldValues[key]) { continue; }

    outputChanges.Add($"{key}: {oldValues[key]} >> {newValues[key]}");
}

if (File.Exists(output)) { File.Delete(output); }

FileStream fs = File.Create(output);
fs.Close();

using (StreamWriter sw = new StreamWriter(output))
{
    sw.WriteLine("UNIQUE KEYS:\n");
    
    foreach(string key in uniqueKeys.Keys) {
        sw.WriteLine($"\t{key}: {uniqueKeys[key]}");
    }

    sw.WriteLine("\nCHANGED KEYS:\n");

    foreach(string change in outputChanges)
    {
        sw.WriteLine($"\t{change}");
    }

    sw.Flush();
}




