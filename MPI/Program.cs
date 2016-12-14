using System;
using System.Collections.Generic;
using System.Text;
using MPI;

namespace MPI
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var mppi = new MPI.Environment(ref args))
            {

                if (Communicator.world.Rank == 0)
                {
                    int n = 5; int m = 5;
                    Random r = new Random();
                    double[,] mas = new double[n,m];
                    for (int i=0;i<n;i++)
                    { for (int j = 0; j < m; j++)
                        {
                            mas[i,j] = r.Next(0, 10);
                        }
                    }
                   
                    Console.WriteLine("Исходный Массив:");
                    for (int i = 0; i < mas.GetLength(0); i++)
                    {
                        for (int j = 0; j < mas.GetLength(1); j++)
                        {
                            Console.Write(mas[i, j] + " ");
                        }
                        Console.WriteLine();
                    }
                    double[] b = new double[n];
                   
                    Console.WriteLine();
                    for (int i = 0; i < n; i++)
                    {
                        for (int j = 0; j <m; j++)
                        {
                            b[i] += mas[j,i];                            
                        }
                    }
                    double currentAmount = 0;
                    double nextAmount = 0;
                    for (int k = 0; k < n; k++)
                    {
                        for (int i = 0; i < n - 1; i++)
                        {
                            for (int j = 0; j < m; j++)
                            {
                                currentAmount += mas[j, i];
                                nextAmount += mas[j, i + 1];
                            }
                            if (nextAmount < currentAmount)
                            {
                                for (int g = 0; g < m; g++)
                                {
                                    var temp = mas[g, i];
                                    mas[g, i] = mas[g, i + 1];
                                    mas[g, i + 1] = temp;
                                }
                            }
                            currentAmount = 0;
                            nextAmount = 0;
                        }
                    }
                
                    Communicator.world.Send(mas, 1, 0);
                }


                if (Communicator.world.Rank == 1)
                {
                    double[,] msg = Communicator.world.Receive<double[,]>(Communicator.world.Rank - 1, 0);
                    Console.WriteLine("Итоговый Массив:");
                    for (int i = 0; i < msg.GetLength(0); i++)
                    {
                        for (int j = 0; j < msg.GetLength(1); j++)
                        {
                            Console.Write(msg[i, j] + " ");
                        }
                        Console.WriteLine();
                    }
                }

            }
        }
    }
}
