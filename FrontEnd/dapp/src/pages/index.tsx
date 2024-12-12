"use client";
import React, { FC, useMemo, useEffect, useState } from "react";
import { Unity, useUnityContext } from "react-unity-webgl";
import Image from "next/image";
import bonktomoon from "/public/bonktomoon.png";
import { WalletMultiButton } from "@solana/wallet-adapter-react-ui";
import { useWallet } from "@solana/wallet-adapter-react";
import { useAnchorWallet, useConnection } from "@solana/wallet-adapter-react";
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
    loaderUrl: "/Build/GameBuild.loader.js",
    dataUrl: "/Build/GameBuild.data",
    frameworkUrl: "/Build/GameBuild.framework.js",
    codeUrl: "/Build/GameBuild.wasm",
  });

  const handleStartRank = () => {
    startGame(program, leaderboardPda, gameSessionPda, wallet, playerName);
  };

  useEffect(() => {
    // Add an event listener to the window to listen for the startGame
    addEventListener("StartRank", handleStartRank);
    return () => {
      removeEventListener("StartRank", handleStartRank);
    };
  }, [addEventListener, removeEventListener, handleStartRank]);

  const handleEndGame = () => {
    endGame(program, leaderboardPda, gameSessionPda, wallet, finalScore);
  };

  useEffect(() => {
    addEventListener("EndGame", handleEndGame);
    return () => {
      removeEventListener("EndGame", handleEndGame);
    };
  }, [addEventListener, removeEventListener, handleEndGame]);

  //Get leaderboard
  const fetchLeaderboard = async () => {
    if (!program || !leaderboardPda) return;
    try {
      const playersList = await getLeaderboard(program, leaderboardPda);
      setLeaderboard(playersList);
      sendMessage("ReactBridge", "sendLeaderboard", playersList);
      console.log("leaderboarddd!:", playersList);
    } catch (error) {
      console.error("Failed to fetch leaderboard:", error);
    }
  };

  // We'll use a state to store the device pixel ratio.
  const [devicePixelRatio, setDevicePixelRatio] = useState(
    typeof window !== "undefined" ? window.devicePixelRatio : 1
  );

  // contract connect part
  const wallet = useAnchorWallet();
  const { connection } = useConnection();
  const { program } = getProgram(
    connection,
    wallet,
    CONFIG.idl,
    CONFIG.programId,
    new PublicKey(CONFIG.ownerTokenAccount)
  );

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

  return (
    <div className="flex flex-col items-center justify-center min-h-screen bg-sky-100">
      {/* Top Bar */}
      <div className="flex justify-between items-center w-full p-2 bg-[#FC7F18]">
        <div className="flex items-center justify-evenly gap-3">
          <Image src={bonktomoon} alt="bonktomoon" width={150} height={100} />
          <button onClick={fetchLeaderboard}>Get Leaderboard</button>
          <button
            onClick={() =>
              addPrizePool(program, leaderboardPda, wallet, addPoolAmount)
            }
          >
            Add Prize Pool
          </button>
          <button
            onClick={() =>
              startGame(
                program,
                leaderboardPda,
                gameSessionPda,
                wallet,
                playerName
              )
            }
          >
            Start Game
          </button>
          <button
            onClick={() =>
              endGame(
                program,
                leaderboardPda,
                gameSessionPda,
                wallet,
                finalScore
              )
            }
          >
            End Game
          </button>
          <button onClick={() => claimPrize(program, leaderboardPda, wallet)}>
            claim Prize
          </button>
        </div>
        <WalletMultiButton
          style={{ backgroundColor: "#121214", borderRadius: "40px" }}
        />
      </div>

      <div className="relative w-full h-auto">
        <Unity
          unityProvider={unityProvider}
          style={{
            height: "calc(100vh - 64px)",
            width: "100%",
          }}
          devicePixelRatio={devicePixelRatio}
        />
      </div>
      <audio src="/bgm.mp3" muted={false} autoPlay loop />
    </div>
  );
};

export default Game;
