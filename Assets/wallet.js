// Connect to the Arweave wallet
async function connectWallet() {
    try {
        await window.arweaveWallet.connect(["ACCESS_ADDRESS", "SIGN_TRANSACTION"]);
        console.log("Wallet connected!");
    } catch (error) {
        console.error("Error connecting wallet:", error);
    }
}

// Get the user's Arweave wallet address
async function getWalletAddress() {
    try {
        let address = await window.arweaveWallet.getActiveAddress();
        console.log("Wallet Address:", address);
        return address;
    } catch (error) {
        console.error("Error fetching wallet address:", error);
    }
}

// Send game data to Arweave
async function saveGameData(data) {
    let arweave = Arweave.init({ host: "arweave.net", protocol: "https" });

    let transaction = await arweave.createTransaction({ data: JSON.stringify(data) });
    transaction.addTag("Content-Type", "application/json");

    await window.arweaveWallet.sign(transaction);
    await arweave.transactions.post(transaction);

    console.log("Game data saved! Tx ID:", transaction.id);
}
