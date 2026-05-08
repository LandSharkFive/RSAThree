# RSAThree: RSA Certificate & Encryption Demo

This project demonstrates how to manage RSA certificates within the Windows Certificate Store (X509Store) and perform secure encryption/decryption.

## Features
1. **Basic RSA:** Simple in-memory encryption/decryption.
2. **Persistence:** Creates a self-signed certificate and persists the Private Key to the user's secure container.
3. **Store Integration:** Reads and displays certificate metadata from the OS.
4. **File Encryption:** Encrypts a string to a binary file using the Public Key.
5. **File Decryption:** Recovers the plaintext from the file using the Private Key.

## The "Persistence" Trick
By default, ".NET" creates ephemeral keys. To ensure the Private Key remains available after the app closes, this demo exports the certificate to a PFX buffer and re-imports it using "X509KeyStorageFlags.PersistKeySet".



## Important Constraints
* **RSA Data Limit:** RSA is designed for small data. For a 2048-bit key using OAEP padding, the maximum input is ~214 bytes.
* **Trust Flags:** Since the certificate is self-signed, we search the store with "validOnly: false". A "true" flag would fail because the certificate is not in the Trusted Root folder.
* **Padding:** This demo uses "OaepSHA256", which is more secure than the older PKCS#1 v1.5 standard.

## Usage
1. Run **Demo Two** first to generate and install "TestCertificate".
2. Run **Demo Three** to verify the certificate and Private Key are present.
3. Use **Demo Four** and **Five** to test file-based encryption.

## Requirements
* .NET 8.0 or higher
* Windows (for X509Store integration)
