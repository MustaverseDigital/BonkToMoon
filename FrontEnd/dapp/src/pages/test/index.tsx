"use client";
import React, { FC, useMemo, useEffect, useState } from "react";
import { Unity, useUnityContext } from "react-unity-webgl";
import Image from "next/image";
import bonktomoon from "/public/bonktomoon.png";
import { WalletMultiButton } from "@solana/wallet-adapter-react-ui";
import { useWallet } from "@solana/wallet-adapter-react";
import { useAnchorWallet, useConnection, AnchorWallet } from "@solana/wallet-adapter-react";
import { CONFIG } from "@/config/config";
import { getProgram } from "@/utils/transaction";
import { PublicKey } from "@solana/web3.js";
import {
  getLeaderboard,
  addPrizePool,
  startGame,
  endGame,
  claimPrize,
} from "@/utils/gameAction";

const Game = () => {
  const [leaderboard, setLeaderboard] = useState([]);
  const { publicKey } = useWallet();
  console.log(publicKey?.toBase58());

  const playerName = publicKey?.toBase58().slice(0, 5);
  const addPoolAmount = 100;
  const finalScore = 7000;

  const { unityProvider, sendMessage } = useUnityContext({
    loaderUrl: "/Build/docs.loader.js",
    dataUrl: "/Build/docs.data",
    frameworkUrl: "/Build/docs.framework.js",
    codeUrl: "/Build/docs.wasm",
  });

  //Get leaderboard
  const fetchLeaderboard = async () => {
    if (!program || !leaderboardPda) return;
    try {
      const playersList = await getLeaderboard(program, leaderboardPda);
      const playersListString = JSON.stringify(playersList);
      setLeaderboard(playersList);
      console.log("leaderboarddd!:", playersListString);
      sendMessage("ReactBridge", "sendLeaderboard", playersListString);
    } catch (error) {
      console.error("Failed to fetch leaderboard:", error);
    }
  };

  // We'll use a state to store the device pixel ratio.
  const [devicePixelRatio, setDevicePixelRatio] = useState(
    typeof window !== "undefined" ? window.devicePixelRatio : 1
  );

  // contract connect part
  const wallet = useAnchorWallet() as AnchorWallet;
  const { connection } = useConnection();
  const { program } = getProgram(
    connection,
    wallet,
    CONFIG.idl,
    CONFIG.programId,
    new PublicKey(CONFIG.ownerTokenAccount)
  );

  sendMessage("ReactBridge", "sendAddress", wallet?.publicKey?.toBase58());

  const [leaderboardPda] = PublicKey.findProgramAddressSync(
    [Buffer.from("leaderboard")],
    program.programId
  );

  const [gameSessionPda] = useMemo(() => {
    if (!wallet?.publicKey) return [null];
    return PublicKey.findProgramAddressSync(
      [Buffer.from("player_session"), wallet.publicKey.toBuffer()],
      program.programId
    );
  }, [wallet, program.programId]);

  useEffect(() => {
    if (typeof window === "undefined") return;
    // A function which will update the device pixel ratio of the Unity
    // Application to match the device pixel ratio of the browser.
    const updateDevicePixelRatio = () => {
      setDevicePixelRatio(window.devicePixelRatio);
    };
    // A media matcher which watches for changes in the device pixel ratio.
    const mediaMatcher = window.matchMedia(
      `screen and (resolution: ${devicePixelRatio}dppx)`
    );
    // Adding an event listener to the media matcher which will update the
    // device pixel ratio of the Unity Application when the device pixel
    // ratio changes.
    mediaMatcher.addEventListener("change", updateDevicePixelRatio);
    return () => {
      // Removing the event listener when the component unmounts.
      mediaMatcher.removeEventListener("change", updateDevicePixelRatio);
    };
  }, [devicePixelRatio]);

  useEffect(() => {
    const handleStartRank = () => {
      console.log("StartRank");
      if (!program || !leaderboardPda || !gameSessionPda || !wallet || !playerName) return;
      startGame(program, leaderboardPda, gameSessionPda, wallet, playerName);
    };
    // Add an event listener to the window to listen for the startGame
    addEventListener("StartRank", handleStartRank);
    return () => {
      removeEventListener("StartRank", handleStartRank);
    };
  }, [gameSessionPda, leaderboardPda, playerName, program, wallet]);

  useEffect(() => {
    const handleEndGame = () => {
      if (!program || !leaderboardPda || !gameSessionPda || !wallet) return;
      endGame(program, leaderboardPda, gameSessionPda, wallet, finalScore);
    };

    addEventListener("EndGame", handleEndGame);
    return () => {
      removeEventListener("EndGame", handleEndGame);
    };
  }, [gameSessionPda, leaderboardPda, program, wallet]);

  return (
    <div className="flex flex-col items-center justify-center min-h-screen bg-sky-100">
      {/* Top Bar */}
      <div className="flex justify-between items-center w-full p-2 bg-[#FC7F18]">
        <div className="flex items-center justify-evenly gap-3">
          <Image src={bonktomoon} alt="bonktomoon" width={150} height={100} />

        </div>
        <WalletMultiButton
          style={{ backgroundColor: "#121214", borderRadius: "40px" }}
        />
      </div>
      <div className="flex flex-col gap-5 p-5 relative w-full h-auto">
        <Unity
          unityProvider={unityProvider}
          style={{
            height: "calc(100vh - 64px)",
            width: "100%",
          }}
          devicePixelRatio={devicePixelRatio}
        />
        <button
          className='bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded'
          onClick={fetchLeaderboard}
        >Get Leaderboard
        </button>
        <button
          className='bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded'
          onClick={() =>
            addPrizePool(program, leaderboardPda, wallet, addPoolAmount)
          }
        >
          Add Prize Pool
        </button>
        <button
          className='bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded'
          onClick={() =>
            startGame(
              program,
              leaderboardPda,
              gameSessionPda as PublicKey,
              wallet,
              playerName as string
            )
          }
        >
          Start Game
        </button>
        <button
          className='bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded'
          onClick={() =>
            endGame(
              program,
              leaderboardPda,
              gameSessionPda as PublicKey,
              wallet,
              finalScore
            )
          }
        >
          End Game
        </button>
        <button
          className='bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded'
          onClick={() => claimPrize(program, leaderboardPda, wallet)}>
          claim Prize
        </button>

      </div>
    </div>
  );
};

export default Game;
