# SimChain 🔗

An educational prototype of a blockchain built entirely in C# to illustrate the mechanics of the Proof-of-Work (PoW) consensus algorithm.

## Overview

SimChain is a pure C# educational blockchain implementation that demonstrates blockchain fundamentals through hands-on interaction. Built from scratch as a .NET Core console application, it focuses on proof-of-work consensus with multi-threaded mining simulation, cryptographic security, and distributed network validation.

## ✨ Features

### 🔐 **Interactive Learning Experience**
- **Add Records**: Input data that gets encrypted and mined into new blocks
- **Read Blocks**: Retrieve and decrypt specific blocks by ID
- **Chain Verification**: Validate blockchain integrity across multiple miners
- **Real-time Mining**: Watch concurrent mining operations in action

### ⛓️ **Pure C# Implementation**
- Built entirely in C# using .NET Core console application
- LinkedList-based blockchain structure for educational clarity
- No external blockchain libraries - pure educational implementation
- Clean, readable code with comprehensive architecture

### 🛠️ **Advanced Mining Simulation**
- **Configurable Complexity**: Set mining difficulty (number of leading zeros)
- **Multi-Miner Network**: Configure multiple concurrent miners
- **Nonce-based Proof-of-Work**: Real mining algorithm implementation
- **Consensus Validation**: 90%+ acceptance rate required for block addition

### 👥 **Distributed Network Features**
- **Multi-threaded Mining**: Concurrent mining across all configured miners
- **Chain Synchronization**: Automatic chain updates based on network consensus
- **Integrity Verification**: Cross-validation between miner blockchain copies
- **Self-Healing Network**: Automatic resolution of chain inconsistencies

### 💰 **Economic Model**
- **Miner Rewards**: Miners earn coins for successful mining operations
- **Dynamic Rewards**: Reward amounts decrease as blockchain grows
- **Chain Scoring**: Miners maintain reputation scores based on consensus participation

## 🏗️ Architecture

### Core Classes

#### **Miner Class**
- `async Task<string[]> mine(string encryptedData, int complexity)` - Mining operation
- `LinkedList<Block> Blockchain` - Local blockchain copy
- `bool verifyChain(LinkedList<Block> Blockchain)` - Chain validation
- `bool verifyBlock(Block block)` - Individual block verification
- `bool updateChain(LinkedList<Block> Blockchain)` - Chain synchronization
- `double chainScore` - Consensus reputation score
- `int id` - Unique miner identifier

#### **Block Class**
- `string Id` - Block hash identifier
- `string prevId` - Previous block hash (linking mechanism)
- `string data` - Encrypted transaction data
- `int nonce` - Proof-of-work solution

#### **BlockBO (Business Operations) Class**
- `AddBlockWrapper addBlock(...)` - Complete block addition workflow
- `string getBlock(string Id, LinkedList<Block> Blockchain)` - Block retrieval
- `string verifyChain(...)` - Network-wide chain verification
- `displayChain(LinkedList<Block> Blockchain)` - Visual chain representation

#### **Program Class**
- Configuration management (miner count, complexity)
- Cryptographic key generation and management
- `string encryptData(string data, string publicKey)` - Data encryption
- `string decryptData(string edata, string privateKey)` - Data decryption
- User interface and menu system

## 🚀 Getting Started

### Quick Start (No Development Setup Required)

If you don't want to tinker with code and just want to run the exe, go to bin/Debug/.net8.0 and run the BlockchainSim.exe to start the program in a terminal.

### Prerequisites

- .NET Core 3.1 or later
- Visual Studio 2019+ or VS Code
- Basic understanding of C# and blockchain concepts

### Installation

1. Clone the repository:
```bash
git clone https://github.com/DiscardedOne/SimChain.git
cd simchain
```

2. Build the project:
```bash
dotnet build
```

3. Run the application:
```bash
dotnet run
```

## 📖 Usage

### Initial Configuration

When you start SimChain, you'll be prompted to configure:

1. **Number of Miners**: How many concurrent miners to simulate (e.g., 3-5)
2. **Complexity Level**: Mining difficulty as number of leading zeros (e.g., 3-5)
3. **Cryptographic Keys**: Provide existing keys or generate new ones

### Main Operations

#### **1. Add Block**
```
> Add Record
> Enter data: "Transfer 50 coins from Alice to Bob"
```
- Data gets encrypted with your public key
- All miners compete to solve proof-of-work
- Block added if >90% miner consensus achieved
- Miners receive rewards for successful mining

#### **2. Read Block**
```
> View Record/Read Block
> Enter Block ID: [block-hash-id]
```
- Retrieves block from blockchain
- Decrypts data using your private key
- Displays original transaction data

#### **3. Verify Chain**
```
> Verify LocalChain
```
- Validates blockchain integrity across all miners
- Updates local chain if consensus indicates discrepancies
- Displays chain status and any updates performed

#### **4. Display Chain**
```
Current Blockchain: [1] -- [2] -- [3] -- [4]
```
- Visual representation of current blockchain state
- Shows number of blocks and linking structure

## 🔧 Technical Implementation

### Mining Process Flow

1. **Block Creation**: New block initialized with data and previous block hash
2. **Concurrent Mining**: All miners simultaneously attempt to find valid nonce
3. **Proof-of-Work**: Miners iterate nonce values until hash meets complexity requirement
4. **Validation**: Block verified against each miner's local blockchain
5. **Consensus**: Block accepted only with >90% miner agreement
6. **Chain Update**: Successful blocks added to all miner chains with >50% acceptance

### Chain Verification Algorithm

1. **Length Check**: Compare blockchain lengths across miners
2. **Hash Validation**: Verify each block hash in sequence
3. **Consensus Calculation**: Determine acceptance percentage
4. **Auto-Healing**: Update chains that fall below 90% consensus
5. **Score Calculation**: Update miner chain scores based on network agreement

### Cryptographic Security

- **RSA Encryption**: Data encrypted with public/private key pairs
- **SHA-256 Hashing**: Block integrity and proof-of-work validation
- **Digital Signatures**: Transaction authenticity verification
- **Nonce-based Mining**: Computational proof-of-work implementation

## 🎯 Learning Objectives

This project demonstrates:

- **Blockchain Fundamentals**: Block creation, linking, and validation
- **Proof-of-Work Mining**: Nonce discovery and difficulty adjustment
- **Distributed Consensus**: How networks agree on valid chains
- **Cryptographic Security**: Encryption, hashing, and key management
- **Multi-threading**: Concurrent programming with async/await patterns
- **Data Structures**: LinkedList implementation for blockchain storage
- **Network Simulation**: Multi-node blockchain behavior

## 🔬 Educational Features

### **Controlled Chaos Testing**
- Introduce artificial inconsistencies to observe self-healing
- Test consensus mechanisms under network stress
- Demonstrate blockchain resilience properties

### **Performance Analytics**
- Mining time measurements across different complexities
- Consensus achievement rates under various configurations
- Chain synchronization efficiency metrics

## 🤝 Contributing

We welcome contributions! Areas for improvement:

- **New Consensus Algorithms**: Implement Proof-of-Stake or other mechanisms
- **Network Simulation**: Enhanced P2P communication patterns
- **GUI Interface**: Visual blockchain explorer
- **Performance Optimization**: Mining efficiency improvements
- **Test Coverage**: Unit tests for core blockchain operations

Please read our [Contributing Guidelines](CONTRIBUTING.md) before submitting pull requests.

## 🙏 Acknowledgments

- Built for educational purposes to make blockchain technology accessible
- Inspired by real-world blockchain implementations
- Thanks to the .NET community for excellent async/threading documentation

---

*Building in public and sharing knowledge - one block at a time!* ⛓️

## 🔖 Tags

`blockchain` `proof-of-work` `csharp` `dotnet-core` `cryptography` `distributed-systems` `education` `mining` `consensus` `multi-threading` `linkedlist` `rsa-encryption`
