using System;
using System.Collections;
using UnityEngine;
using System.IO;

public static class Utilities{

    public static void Stringify2DArray(float[,] array, int borderX, int borderY){
        String result = "";
        for (int i = 0; i < borderX; i++)
        {
            String row = "|"; 
            String rowsplitter = "|"; 
            for (int j = 0; j < borderY; j++)
            {
                row += array[i,j] + "|";
                rowsplitter += "-|";
            }
            result += row + "\n" + rowsplitter + "\n";
        }

        Debug.Log(result);
    }


    public static void ExportArrayToCSV(float[] dataArray, String headers, string filePath)
    {
        filePath = "./Metrics/" + filePath;
        try{
            Directory.CreateDirectory("./Metrics");
        } catch(SystemException e){
            // it's alright, then the directory already exists
        }
        
        // Create a string builder to store CSV data
        System.Text.StringBuilder csvContent = new System.Text.StringBuilder();

        // Append header if needed
        csvContent.AppendLine(headers);

        // Append each data item
        foreach (object data in dataArray)
        {
            csvContent.AppendLine(data.ToString());
        }

        // Write CSV data to file
        File.WriteAllText(filePath, csvContent.ToString());

        Debug.Log("CSV file exported to: " + filePath);
    }

    public static void ExportArrayToCSV(int[] dataArray, String headers, string filePath)
    {
        filePath = "./Metrics/" + filePath;
        try{
            Directory.CreateDirectory("./Metrics");
        } catch(SystemException e){
            // it's alright, then the directory already exists
        }

        // Create a string builder to store CSV data
        System.Text.StringBuilder csvContent = new System.Text.StringBuilder();

        // Append header if needed
        if (headers != "") csvContent.AppendLine(headers);

        // Append each data item
        foreach (object data in dataArray)
        {
            csvContent.AppendLine(data.ToString());
        }

        // Write CSV data to file
        File.WriteAllText(filePath, csvContent.ToString());

        Debug.Log("CSV file exported to: " + filePath);
    }

    public static void AppendLineToFile(String filePath, String lineToAppend)
    {
        filePath = "./Metrics/" + filePath;
        try{
            Directory.CreateDirectory("./Metrics");
        } catch(SystemException e){
            // it's alright, then the directory already exists
        }

        try
        {
            // Check if the file exists
            if (File.Exists(filePath))
            {
                // Open the file and append the line
                using (StreamWriter sw = File.AppendText(filePath))
                {
                    sw.WriteLine(lineToAppend);
                }
            }
            else
            {
                Debug.LogError("File does not exist");
            }
        }
        catch (IOException e)
        {
            Debug.LogError("An IO exception occurred: " + e.Message);
        }
        catch (System.Exception e)
        {
            Debug.LogError("An exception occurred: " + e.Message);
        }
    }
}