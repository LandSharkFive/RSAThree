# Lesson Plan: RSA Efficiency & BigInteger Math
**Duration:** 90 Minutes  
**Prerequisites:** Familiarity with the basic RSA workflow ($p, q, n, e, d$).  
**Objective:** Students will analyze the computational cost of modular inversion and understand why `BigInteger` is necessary for cryptographic security.

---

### 1. The Scaling Problem: Why `int` Isn't Enough (15 mins)
* **Concept:** Standard 32-bit integers max out at $2,147,483,647$. In RSA, the modulus $n$ can easily have 600+ digits.
* **Discussion:** Ask students what happens if we calculate $12345^{67890}$ in a standard calculator. (Answer: `Overflow` or `Infinity`).
* **BigInteger Intro:** Explain that `System.Numerics.BigInteger` in C# (used in RSAThree) treats numbers like "arrays" of data, allowing them to grow as large as the computer's memory permits.



### 2. Finding the Secret Key: The `ModInv()` Function (25 mins)
* **The Math:** To find the private key $d$, we must solve for the modular multiplicative inverse:
    $$e \cdot d \equiv 1 \pmod{\phi(n)}$$
* **The Brute Force Approach:**
    * Imagine trying every number starting from 1 until the equation works. 
    * *Exercise:* If $\phi(n)$ is a 200-digit number, and your computer checks 1 billion numbers per second, the sun would burn out before you found $d$.
* **The Efficient Way:** The **Extended Euclidean Algorithm**. It finds the inverse by working backward from the GCD, "jumping" to the answer in a few dozen steps.



### 3. Lab Activity: Benchmarking `ModInv` (30 mins)
Using the [RSAThree/RSA.cs](https://github.com/LandSharkFive/RSAThree/blob/master/RSA.cs) file:
* **Code Analysis:** Locate the `ModInv(BigInteger e, BigInteger n)` method. 
* **Experiment:** 1.  Have students write a simple `for` loop to find a modular inverse for small primes.
    2.  Provide them with 20-digit primes. Ask them to run the brute force loop. (The program will likely hang).
    3.  Switch to the `ModInv` provided in the repo. It will finish instantly.
* **Key Takeaway:** Algorithmic complexity (Big O) is the difference between a secure system and a broken one.

### 4. Computational Shortcuts: `ModPow` (20 mins)
* **Concept:** RSA encryption is $c = m^e \pmod{n}$. 
* **The Trick:** You don't actually calculate $m^e$ first (which would be trillions of digits long). You use **Square-and-Multiply**.
* **Code Observation:** Look at how `BigInteger.ModPow` is used in the `Encrypt` and `Decrypt` methods. 



---

### Classroom Resources
* **Repository:** [LandSharkFive/RSAThree](https://github.com/LandSharkFive/RSAThree)
* **Documentation:** [Microsoft Docs: BigInteger Structure](https://learn.microsoft.com/en-us/dotnet/api/system.numerics.biginteger)
* **Math Check:** [Omni Calculator - Modular Multiplicative Inverse](https://www.omnicalculator.com/math/modular-inverse)

---

### Assessment Questions
1.  Why is the runtime of `ModInv` critical for the person *generating* the keys, but not necessarily for the person *sending* a message?
2.  If you had to implement RSA without the `BigInteger` library, what is the biggest challenge you would face?