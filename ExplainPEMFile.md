## Understanding PEM Files

A **PEM (Privacy-Enhanced Mail)** file is a text-based format used to store cryptographic objects like **X.509 certificates, private keys, or public keys**. It uses **Base64 encoding** to represent binary data (like DER-encoded ASN.1) in a text-safe way.
   

## Decoding the Provided X.509 Certificate

PEM files are wrapped with headers like:

- **`type`**: `X.509` — standard for PKI certificates.
- **`version`**: `3` — supports extensions (e.g., SANs).
- **`SerialNumber`**: `00:F6:F5:95:09:1E:9D:B0:30` — unique ID.
- **`SigAlgName`**: `SHA256withRSA` — signature algorithm.
- **`NotBefore` / `NotAfter`**: Validity period.
- **`SubjectDN` / `IssuerDN`**: Both `CN=TestCertificate` → **self-signed**.
- **`encoded`**: DER-encoded ASN.1 data in **hex**. In PEM, this would be **Base64**.
- **`signature`**: Digital signature in hex.
- **`isSelfSigned`**: Confirmed.

The "encoded" field is a **hex dump** of the DER structure.