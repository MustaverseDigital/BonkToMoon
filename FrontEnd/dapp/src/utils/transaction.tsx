import * as anchor from "@coral-xyz/anchor";
import { Program, AnchorProvider } from "@coral-xyz/anchor";
import { PublicKey, Connection } from "@solana/web3.js";
import { AnchorWallet } from "@solana/wallet-adapter-react";

export const getProgram = (
  connection: Connection,
  wallet: AnchorWallet,
  idl: any,
  contractAddress: string,
  authority: PublicKey // Add authority parameter
) => {
  const provider = new AnchorProvider(connection, wallet, {
    commitment: "confirmed",
  });
  anchor.setProvider(provider);

  const program: Program<programType> = new Program(
    idl as any,
    contractAddress,
    provider
  );

  return { program, provider, authority }; // Return authority if needed
};
