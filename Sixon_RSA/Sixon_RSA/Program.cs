using System;
using System.Collections.Generic;


namespace Sixon_RSA
{
    internal class Program
    {
        static void Main(string[] args)
        {
            RSAalgorithm.Run();
            Console.ReadLine(); // Pause before exit
        }
    }

    class RSAalgorithm
    {
        static List<int> primeNumbers = new List<int>
        {
            2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43,
            47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97, 101,
            103, 107, 109, 113, 127, 131, 137, 139, 149,
            151, 157, 163, 167, 173, 179, 181, 191, 193,
            197, 199, 211, 223, 227, 229, 233, 239, 241, 251
        };

        static Random rnd = new Random();

        public static void Run()
        {
            //Select two different random primes
            int p = primeNumbers[rnd.Next(primeNumbers.Count)];
            int q;
            do
            {
                q = primeNumbers[rnd.Next(primeNumbers.Count)];
            } while (q == p);

            //Compute n and totient
            long n = (long)p * q;
            long totient = (long)(p - 1) * (q - 1);

            //Choose Public Key (PuK) and Private Key (PrK)
            long PuK, PrK;
            try
            {
                PuK = GetCoprime(totient);
                PrK = ModInverse(PuK, totient);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return; // Stop the rest of the program if no valid public key is found
            }

            // Display keys
            Console.WriteLine("Key Generation");
            Console.WriteLine("Randomize Selected Primes: P = " + p + ", Q = " + q);
            Console.WriteLine("n (P * Q) = " + n);
            Console.WriteLine("Totient = " + totient);
            Console.WriteLine("Public Key (PuK) = " + PuK);
            Console.WriteLine("Private Key (PrK) = " + PrK);

            //Input message
            Console.Write("Enter A Number To Encrypt: ");
            long message = long.Parse(Console.ReadLine());

            //Encrypt: Ciphertext = Message^PuK mod n
            long ciphertext = ModPow(message, PuK, n);
            Console.WriteLine("Encrypted message: " + ciphertext);

            //Decrypt: Message = Ciphertext^PrK mod n
            long decrypted = ModPow(ciphertext, PrK, n);
            Console.WriteLine("Decrypted message: "+ decrypted);
        }


        //Calculate Valid Public Key(Puk) 
        static long GetCoprime(long totient)
        {
            for (long Puk = 3; Puk < totient; Puk += 2)
            {
                if (GCD(Puk, totient) == 1)
                    return Puk;
            }
            throw new Exception("No valid public key found.");
        }

        //Calculates the GCD
        static long GCD(long a, long b)
        {
            while (b != 0)
            {
                long temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        //To Find the Value of Private Key(Prk)
        static long ModInverse(long PuK, long totient)
        {
            long originalTotient = totient;
            long quotient, temp;
            long previousCoefficient = 0;
            long currentCoefficient = 1;

            if (totient == 1) return 0;

            while (PuK > 1)
            {
                quotient = PuK / totient;

                // Swap values similar to Euclidean Algorithm
                temp = totient;
                totient = PuK % totient;
                PuK = temp;
                    
                // Update coefficients
                temp = previousCoefficient;
                previousCoefficient = currentCoefficient - quotient * previousCoefficient;
                currentCoefficient = temp;
            }

            // Ensure result is positive
            if (currentCoefficient < 0)
                currentCoefficient += originalTotient;

            return currentCoefficient;
        }

        //Modular Exponentiation
        static long ModPow(long baseVal, long exponent, long modulus)
        {
            long result = 1;
            baseVal %= modulus;

            while (exponent > 0)
            {
                if ((exponent & 1) == 1)
                    result = (result * baseVal) % modulus;

                baseVal = (baseVal * baseVal) % modulus;
                exponent >>= 1;
            }

            return result;
        }
    }
}
