using System;
using System.Collections;
using UnityEngine;
using System.IO;

public static class Utilities
{
    public static void InitLocalMetrics(){
        string filePath = "." + Path.DirectorySeparatorChar + "Metrics_" + GameManagement.instance + Path.DirectorySeparatorChar + "actions_executed.json";
        try{
            Directory.CreateDirectory("." + Path.DirectorySeparatorChar + "Metrics_" + GameManagement.instance);
        } catch (SystemException){
            // it's alright, then the directory already exists
        }
        ExecutedData initData = new ExecutedData(){executed = true};
        File.WriteAllText(filePath, JsonUtility.ToJson(initData, false));
    }

    public static void ExportArrayToCSV(String firstRow, String headers, string filePath)
    {
        filePath = "." + Path.DirectorySeparatorChar + "Metrics_" + GameManagement.instance + Path.DirectorySeparatorChar + filePath;
        try{
            Directory.CreateDirectory("." + Path.DirectorySeparatorChar + "Metrics_" + GameManagement.instance);
        } catch (SystemException){
            // it's alright, then the directory already exists
        }
        
        // Create a string builder to store CSV data
        System.Text.StringBuilder csvContent = new System.Text.StringBuilder();

        // Append header if needed
        if (headers != "") csvContent.AppendLine(headers);

        // Append each data item
        if (headers != "") csvContent.AppendLine(firstRow);

        // Write CSV data to file
        File.WriteAllText(filePath, csvContent.ToString());

        Debug.Log("CSV file exported to: " + filePath);
    }

    public static void AppendLineToFile(String filePath, String lineToAppend)
    {
        filePath = "." + Path.DirectorySeparatorChar + "Metrics_" + GameManagement.instance + Path.DirectorySeparatorChar + filePath;
        try{
            Directory.CreateDirectory("." + Path.DirectorySeparatorChar + "Metrics_" + GameManagement.instance);
        } catch(SystemException){
            // it's alright, then the directory already exists
        }

        try{
            // Check if the file exists
            if (File.Exists(filePath)){
                // Open the file and append the line
                using (StreamWriter sw = File.AppendText(filePath))
                {
                    sw.WriteLine(lineToAppend);
                }
            } else{
                Debug.Log("File does not exist");
            }
        }
        catch (IOException e){
            Debug.Log("An IO exception occurred: " + e.Message);
        } 
        catch (System.Exception e){
            Debug.Log("An exception occurred: " + e.Message);
        }
    }

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
}