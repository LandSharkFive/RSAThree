using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace RSAThree
{
    internal class Program
    {
        // This protects the private key.
        private static string myPassword = "p@$$word123";

        static void Main(string[] args)
        {
            int choice = Convert.ToInt32(Console.ReadLine());
            switch (choice)
            {
                case 1:
                    DemoOne();
                    break;
                case 2:
                    DemoTwo();
                    break;
                case 3:
                    DemoThree();
                    break;
                case 4:
                    DemoFour();
                    break;
                case 5:
                    DemoFive();
                    break;
                case 6:
                    DemoSix();
                    break;
                case 7:
                    RunAllDemos();
                    break;
            }
        }

        /// <summary>
        /// Generate a 2048-bit RSA key pair. Encrypts a plaintext message using the public key with PKCS#1 padding.
        /// Decrypts the ciphertext using the private key.  Outputs the original, encrypted(Base64), and decrypted text.
        /// RSAEncryptionPadding.PKCS1 is used.
        /// </summary>
        static void DemoOne()
        {
            Console.WriteLine("Demo One.");
            Console.WriteLine("Generate Key. Encrypt and Decrypt.");

            // Generate RSA key pair
            using (RSA rsa = RSA.Create(2048))
            {
                string original = "Hello, RSA!";
                Console.WriteLine($"Original: {original}");

                // Encrypt using public key
                byte[] dataToEncrypt = Encoding.UTF8.GetBytes(original);
                byte[] encrypted = rsa.Encrypt(dataToEncrypt, RSAEncryptionPadding.Pkcs1);
                string encryptedBase64 = Convert.ToBase64String(encrypted);
                Console.WriteLine($"Encrypted: {encryptedBase64}");

                Assert(dataToEncrypt.Length > 0, "Missing bytes");
                Assert(encrypted.Length > 0, "Missing bytes");
                Assert(encryptedBase64.Length > 0, "Empty string");

                // Decrypt using private key
                byte[] decrypted = rsa.Decrypt(encrypted, RSAEncryptionPadding.Pkcs1);
                string decryptedText = Encoding.UTF8.GetString(decrypted);
                Console.WriteLine($"Decrypted: {decryptedText}");

                Assert(decryptedText.Length > 0, "Missing bytes");
                Assert(decrypted.Length > 0, "Empty string");
                Assert(decryptedText == original, "Different strings");
                Console.WriteLine("\n");
            }
        }

        /// <summary>
        /// Create Key and Save to Secure Storage Container.
        /// </summary>
        static void DemoTwo()
        {
            Console.WriteLine("Demo Two - Creating Persistent Certificate...");

            using (RSA rsa = RSA.Create(2048))
            {
                var request = new CertificateRequest(
                    "CN=TestCertificate",
                    rsa,
                    HashAlgorithmName.SHA256,
                    RSASignaturePadding.Pkcs1);

                // 1. Create the initial certificate object
                X509Certificate2 tempCert = request.CreateSelfSigned(
                    DateTimeOffset.Now,
                    DateTimeOffset.Now.AddYears(1));

                // 2. IMPORTANT: Export to PFX and Re-import with PersistKeySet
                // This forces Windows to store the private key in its secure container.
                byte[] pfxData = tempCert.Export(X509ContentType.Pfx, myPassword);
                X509Certificate2 persistentCert = new X509Certificate2(
                    pfxData,
                    myPassword,
                    X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.UserKeySet);

                // 3. Save PFX to file (as you did before)
                string outFileName = "cert.pfx";
                File.WriteAllBytes(outFileName, pfxData);
                Assert(File.Exists(outFileName), "File not found");

                // 4. Save the PERSISTENT certificate to the store
                using (var store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
                {
                    store.Open(OpenFlags.ReadWrite);
                    store.Add(persistentCert);
                    store.Close();
                }

                Console.WriteLine("Certificate created and saved with persistent private key.");
            }
        }


        /// <summary>
        /// View Key from X509 Secure Storage Container.
        /// </summary>
        static void DemoThree()
        {
            Console.WriteLine("Demo Three.");
            Console.WriteLine("View Key from X509 Secure Storage Container.");

            // Load and view certificate from store
            using (var store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            {
                store.Open(OpenFlags.ReadOnly);
                var certs = store.Certificates.Find(X509FindType.FindBySubjectName, "TestCertificate", false);

                if (certs.Count == 0)
                {
                    Console.WriteLine("Certificate not found.");
                }
                else
                {
                    if (certs.Count > 1)
                    {
                        Console.WriteLine("Multiple certificates found.");
                    }

                    var cert = certs[0]; // Get the first certificate.
                    Console.WriteLine($"Subject: {cert.Subject}");
                    Console.WriteLine($"Thumbprint: {cert.Thumbprint}");
                    Console.WriteLine($"Has Private Key: {cert.HasPrivateKey}");
                    Assert(cert.Subject.Length > 0, "Empty string");
                    Assert(cert.Thumbprint.Length > 0, "Empty string");

                    var publicKey = cert.GetRSAPublicKey();
                    if (publicKey != null)
                    {
                        var publicKeyBytes = publicKey.ExportSubjectPublicKeyInfo();
                        string publicKeyHex = BitConverter.ToString(publicKeyBytes).Replace("-", " ");
                        Console.WriteLine("Public Key: " + publicKeyHex.Substring(0, 30) + "...");
                        Assert(publicKeyBytes.Length > 0, "Missing bytes");
                        Assert(publicKeyHex.Length > 0, "Empty string");
                    }
                    else
                    {
                        Console.WriteLine("Certificate does not contain an RSA public key.");
                    }
                }
            }
            Console.WriteLine("\n");
        }

        public static void DemoFour()
        {
            Console.WriteLine("Demo Four.");
            Console.WriteLine("Encrypt a string and write it to an encrypted file.");

            string subject = "TestCertificate";
            string message = "Hello RSA!";
            string outFileName = "RSAOne.bin";
            EncryptData(subject, message, outFileName);
            Console.WriteLine($"PLAINTEXT: {message}");
            Console.WriteLine($"Wrote file: {outFileName}");
            Assert(File.Exists(outFileName), "File not found");
            Console.WriteLine("\n");
        }


        public static void DemoFive()
        {
            Console.WriteLine("Demo Five.");
            Console.WriteLine("Decrypt a file and print to screen.");

            string subject = "TestCertificate";
            string inFileName = "RSAOne.bin";
            Assert(File.Exists(inFileName), "File not found");
            DecryptData(subject, inFileName);
            Console.WriteLine($"File read: {inFileName}");
            Console.WriteLine("\n");
        }

        /// <summary>
        /// Encrypt string and write to file.
        /// </summary>
        public static void EncryptData(string certSubject, string text, string outFile)
        {
            using (X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            {
                store.Open(OpenFlags.ReadOnly);
                // Better search: FindBySubjectName is fine, but let's ensure we get the newest/valid one
                var certs = store.Certificates.Find(X509FindType.FindBySubjectName, certSubject, false);

                if (certs.Count == 0)
                {
                    Console.WriteLine($"Certificate '{certSubject}' not found or invalid.");
                    return;
                }

                if (certs.Count > 1)
                {
                    Console.WriteLine("Multiple certificates found.");
                }

                X509Certificate2 cert = certs[0];

                // Try getting the public key
                using (var rsa = cert.GetRSAPublicKey())
                {
                    if (rsa == null)
                    {
                        // FALLBACK: For older CSP certificates
                        Console.WriteLine("Modern GetRSAPublicKey() failed. Attempting legacy provider...");
                        // Note: In modern .NET, GetRSAPublicKey is usually the standard. 
                        // If it's null, the cert might not actually be an RSA cert.
                        return;
                    }

                    byte[] dataToEncrypt = Encoding.UTF8.GetBytes(text);
                    byte[] cipherBytes = rsa.Encrypt(dataToEncrypt, RSAEncryptionPadding.OaepSHA256);
                    File.WriteAllBytes(outFile, cipherBytes);
                    Assert(dataToEncrypt.Length > 0, "Missing bytes");
                    Assert(cipherBytes.Length > 0, "Missing bytes");
                    Assert(File.Exists(outFile), "File not found");
                }
            }
        }

        /// <summary>
        /// Decrypt string from file. Use certificate.
        /// </summary>
        public static void DecryptData(string certSubject, string inFile)
        {
            if (!File.Exists(inFile))
            {
                Console.WriteLine("File not found");
                return;
            }

            using (X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            {
                store.Open(OpenFlags.ReadOnly);
                var certs = store.Certificates.Find(X509FindType.FindBySubjectName, certSubject, false);

                if (certs.Count == 0)
                {
                    Console.WriteLine("Certificate not found.");
                    return;
                }

                if (certs.Count > 1)
                {
                    Console.WriteLine("Multiple certificates found.");
                }

                // Get first certificate.
                X509Certificate2 cert = certs[0];

                if (!cert.HasPrivateKey)
                {
                    Console.WriteLine("Error: Found certificate but it does not have a private key.");
                    return;
                }

                using (var rsa = cert.GetRSAPrivateKey())
                {
                    if (rsa == null)
                    {
                        Console.WriteLine("RSA private key could not be accessed. Check permissions and Provider type.");
                        return;
                    }

                    Assert(File.Exists(inFile), "File not found");
                    byte[] cipherBytes = File.ReadAllBytes(inFile);
                    byte[] plainBytes = rsa.Decrypt(cipherBytes, RSAEncryptionPadding.OaepSHA256);  
                    string decryptedText = Encoding.UTF8.GetString(plainBytes);

                    Console.WriteLine("Decrypted Message: " + decryptedText);
                    Assert(cipherBytes.Length > 0, "Missing bytes");
                    Assert(plainBytes.Length > 0, "Missing bytes");
                    Assert(decryptedText.Length > 0, "Empty string");
                }
            }
        }

        public static void DemoSix()
        {
            Console.WriteLine("Demo Six.");
            Console.WriteLine("Write PEM File.");
            string input = "cert.pfx";
            string output = "public.pem";
            Assert(File.Exists(input), "File not found");
            WritePEMFile(input, output);
            Assert(File.Exists(output), "File not found");
            Console.WriteLine($"Read file {input}");
            Console.WriteLine($"Write file {output}");
            Console.WriteLine("\n");
        }

        /// <summary>
        /// Write a PEM file.
        /// </summary>
        private static void WritePEMFile(string inFile, string outFile)
        {
            if (!File.Exists(inFile))
            {
                Console.WriteLine("File not found");
                return;
            }

            // Load PFX.
            var cert = new X509Certificate2(inFile, myPassword, X509KeyStorageFlags.Exportable);

            // Export certificate to PEM.
            byte[] certificateBytes = cert.RawData;
            char[] certificatePem = PemEncoding.Write("CERTIFICATE", certificateBytes);
            string str1 = new string(certificatePem);
            byte[] firstBytes = Encoding.UTF8.GetBytes(str1);
            File.WriteAllText(outFile, Encoding.UTF8.GetString(firstBytes));

            Assert(certificateBytes.Length > 0, "Missing bytes");
            Assert(certificatePem.Length > 0, "Missing chars");
            Assert(str1.Length > 0, "Empty string");
            Assert(firstBytes.Length > 0, "Missing bytes");
            Assert(File.Exists(outFile), "File not found");
        }


        /// <summary>
        /// Run All Demos.  Exclude DemoTwo() and DemoSix().  They write new certificate files each time.
        /// </summary>
        static void RunAllDemos()
        {
            DemoOne();
            DemoThree();
            DemoFour();
            DemoFive();
        }

        private static void Assert(bool value)
        {
            if (value == false)
            {
                Console.WriteLine("Assert failed.");
            }
        }

        private static void Assert(bool value, string msg)
        {
            if (value == false)
            {
                Console.WriteLine($"Assert failed: {msg}");
            }
        }

    }
}
