using System;
using System.Diagnostics;
using System.Numerics;

public class Program
{
    /// <summary>
    /// This programs helps to estimate the runtime for ModInv(). The program shows the runtime difference between Brute Force and Euclid's algorithm.
    /// </summary>
    public static void Main()
    {
        // Example: e = 7, phi = 40 (Small scale)
        // Try these larger values to see Brute Force fail:
        BigInteger e = 65537;
        BigInteger phi = BigInteger.Parse("123456789012345678901234567890");

        Stopwatch sw = new Stopwatch();

        // 1. Brute Force Attempt (This will take "forever" on large numbers)
        Console.WriteLine("Starting Brute Force...");
        sw.Start();
        // Brute Force #1:  for (BigInteger d = 1; d < phi; d++) { if ((e * d) % phi == 1) { Console.WriteLine("Found"); } }
        // Brute Force #2:  Pick two large random numbers.  Use them in ModInv().  What is the runtime?
        Console.WriteLine("Brute force skipped for safety on large numbers!");

        // 2. Efficient ModInv (Extended Euclid)
        sw.Restart();
        BigInteger result = ModInv(e, phi);
        sw.Stop();
        Console.WriteLine($"Extended Euclid Result: {result}");
        Console.WriteLine($"Time taken: {sw.Elapsed.TotalMilliseconds}ms");
    }

    // This is the logic found in RSAThree
    public static BigInteger ModInv(BigInteger e, BigInteger n)
    {
        BigInteger n0 = n, t, q;
        BigInteger x0 = 0, x1 = 1;

        if (n == 1) return 0;

        while (e > 1)
        {
            q = e / n;
            t = n;
            n = e % n;
            e = t;
            t = x0;
            x0 = x1 - q * x0;
            x1 = t;
        }

        if (x1 < 0) x1 += n0;
        return x1;
    }
}