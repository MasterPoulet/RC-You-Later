using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor.Overlays;
using System.Text;

public static class Timer
{
    private static readonly Stopwatch stopwatch = new();
    private static List<long> steps = new();

    public static bool IsRunning
    {
        get => stopwatch.IsRunning;
    }

    public static double ElapsedSeconds
    {
        get => stopwatch.ElapsedMilliseconds * 0.001f;
    }

    public static int StepsCount
    {
        get => steps.Count;
    }

    public static double GetStepElapsedSeconds(int index)
    {
        return steps[index] * 0.001f;
    }

    /// <summary>
    /// Reset the timer and remove any steps.
    /// </summary>
    public static void Reset()
    {
        stopwatch.Reset();
        steps.Clear();
    }

    public static void Start()
    {
        stopwatch.Start();
    }

    public static void Stop()
    {
        stopwatch.Stop();
    }

    public static void Step()
    {
        steps.Add(stopwatch.ElapsedMilliseconds);
    }

    // Création du fichier de sauvegarde + son chemin
    private static readonly string SavePath = Path.Combine(
    UnityEngine.Application.dataPath, "..", "score.txt");

    public static void Save()
    {
        if (steps.Count == 0) return;

        // Temps final
        long newFinalTime = steps[steps.Count - 1];

        // Meilleur temps
        if (File.Exists(SavePath))
        {
            List<long> existingSteps = LoadStepsFromFile();
            if (existingSteps != null && existingSteps.Count > 0)
            {
                long oldFinalTime = existingSteps[existingSteps.Count - 1];
                if (newFinalTime >= oldFinalTime)
                {
                    return;
                }
            }
        }

        // Construire la chaîne : une valeur par ligne
        StringBuilder sb = new StringBuilder();
        foreach (long step in steps)
        {
            sb.AppendLine(step.ToString());
        }

        // Mis en place du système de sauvegarde en base64 pour éviter les changement manuels du fichier de sauvegarde
        string plainText = sb.ToString();
        string encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));

        File.WriteAllText(SavePath, encoded);
    }


    public static void Load()
    {
        List<long> loaded = LoadStepsFromFile();
        if (loaded != null)
        {
            steps = loaded;
        }
    }

    private static List<long> LoadStepsFromFile()
    {
        if (!File.Exists(SavePath)) return null;

        try
        {
            string encoded = File.ReadAllText(SavePath);

            // Décodage depuis fichier qui est en Base64
            string plainText = Encoding.UTF8.GetString(Convert.FromBase64String(encoded));

            List<long> loaded = new List<long>();
            string[] lines = plainText.Split(
                new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries
            );

            foreach (string line in lines)
            {
                if (long.TryParse(line, out long value))
                {
                    loaded.Add(value);
                }
            }

            return loaded.Count > 0 ? loaded : null;
        }
        catch
        {
            // Fichier corrompu ou invalide
            return null;
        }
    }
}
