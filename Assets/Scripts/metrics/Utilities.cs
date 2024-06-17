using System;
using System.Collections;
using UnityEngine;

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
}