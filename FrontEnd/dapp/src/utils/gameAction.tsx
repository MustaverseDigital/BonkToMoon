import { PublicKey } from "@solana/web3.js";
import * as anchor from "@coral-xyz/anchor";
import { AnchorWallet } from "@solana/wallet-adapter-react";
import { TOKEN_PROGRAM_ID, getAssociatedTokenAddress } from "@solana/spl-token";

export const getLeaderboard = async (program: anchor.Program, leaderboardPda: PublicKey) => {
  const leaderboardAccount = await program.account.leaderboard.fetch(leaderboardPda);

  const playersList = leaderboardAccount.players.map((player: any) => ({
    name: player.name,
    score: player.score,
    address: player.address.toBase58(),
  }));

  return playersList;
};

export const addPrizePool = async (
  program: anchor.Program,
  leaderboardPda: PublicKey,
  wallet: AnchorWallet,
  amount: number
) => {
  const leaderboardAccount = await program.account.leaderboard.fetch(leaderboardPda);
  const contributorTokenAccount = await getAssociatedTokenAddress(
    leaderboardAccount.tokenMint,
    wallet.publicKey
  );
  const tokenPoolAccount = await getAssociatedTokenAddress(
    leaderboardAccount.tokenMint,
    leaderboardPda,
    true
  );

  console.log("Contributor Token Account:", contributorTokenAccount.toString());
  console.log("Token Pool Account:", tokenPoolAccount.toString());

  const tx = await program.methods
    .addPrizePool(new anchor.BN(amount))
    .accounts({
      leaderboard: leaderboardPda,
      contributorTokenAccount,
      tokenPool: tokenPoolAccount,
      contributor: wallet.publicKey,
      tokenProgram: TOKEN_PROGRAM_ID,
    })
    .rpc();

  console.log("add prize Transaction:", tx);
};

export const startGame = async (
  program: anchor.Program,
  leaderboardPda: PublicKey,
  gameSessionPda: PublicKey,
  wallet: AnchorWallet,
  gameName: string
) => {
  const leaderboardAccount = await program.account.leaderboard.fetch(leaderboardPda);
  const contributorTokenAccount = await getAssociatedTokenAddress(
    leaderboardAccount.tokenMint,
    wallet.publicKey
  );
  const tokenPoolAccount = await getAssociatedTokenAddress(
    leaderboardAccount.tokenMint,
    leaderboardPda,
    true
  );

  console.log("Game Session PDA:", gameSessionPda.toString());
  console.log("Token Pool Account:", tokenPoolAccount.toString());

  const tx = await program.methods
    .startGame(gameName)
    .accounts({
      leaderboard: leaderboardPda,
      gameSession: gameSessionPda,
      payerTokenAccount: contributorTokenAccount,
      tokenPool: tokenPoolAccount,
      payer: wallet.publicKey,
      systemProgram: anchor.web3.SystemProgram.programId,
      tokenProgram: TOKEN_PROGRAM_ID,
    })
    .rpc();

  console.log("Start game Transaction:", tx);
};

export const endGame = async (
  program: anchor.Program,
  leaderboardPda: PublicKey,
  gameSessionPda: PublicKey,
  wallet: AnchorWallet,
  score: number
) => {
  const tx = await program.methods
    .endGame(new anchor.BN(score))
    .accounts({
      leaderboard: leaderboardPda,
      gameSession: gameSessionPda,
      payer: wallet.publicKey,
    })
    .rpc();

  console.log("Transaction:", tx);
  console.log("End Game");
};

export const claimPrize = async (
  program: anchor.Program,
  leaderboardPda: PublicKey,
  wallet: AnchorWallet,
) => {
  
  const leaderboardAccount = await program.account.leaderboard.fetch(leaderboardPda);
  const contributorTokenAccount = await getAssociatedTokenAddress(
    leaderboardAccount.tokenMint,
    wallet.publicKey
  );
  const tokenPoolAccount = await getAssociatedTokenAddress(
    leaderboardAccount.tokenMint,
    leaderboardPda,
    true
  );
  const tx = await program.methods
    .claimPrize()
    .accounts({
      leaderboard: leaderboardPda,
      tokenPool: tokenPoolAccount,
      playerTokenAccount: contributorTokenAccount,
      player: wallet.publicKey,
      tokenProgram: TOKEN_PROGRAM_ID,
    })
    .rpc();
  console.log("Claim Transaction:", tx);
  console.log("claimPrize");
};
