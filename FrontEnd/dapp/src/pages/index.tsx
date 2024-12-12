"use client";
import React, { useMemo, useEffect, useState } from "react";
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
  startGame,
  endGame,
} from "@/utils/gameAction";

const Game = () => {
  const [leaderboard, setLeaderboard] = useState([]);
  const { publicKey } = useWallet();
  console.log(publicKey?.toBase58());

  const playerName = publicKey?.toBase58().slice(0, 5);
  const addPoolAmount = 100;
  const finalScore = 7000;

  const { unityProvider, sendMessage, isLoaded } = useUnityContext({
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
      console.log('leaderboard', leaderboard);
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

    const updateDevicePixelRatio = () => {
      setDevicePixelRatio(window.devicePixelRatio);
    };
    const mediaMatcher = window.matchMedia(
      `screen and (resolution: ${devicePixelRatio}dppx)`
    );

    mediaMatcher.addEventListener("change", updateDevicePixelRatio);
    return () => {
      mediaMatcher.removeEventListener("change", updateDevicePixelRatio);
    };
  }, [devicePixelRatio]);

  useEffect(() => {
    const handleStartRank = () => {
      console.log("StartRank");
      if (!program || !leaderboardPda || !gameSessionPda || !wallet || !playerName) return;
      startGame(program, leaderboardPda, gameSessionPda as PublicKey, wallet, playerName);
    };
    // Add an event listener to the window to listen for the startGame
    addEventListener("StartRank", handleStartRank);
    return () => {
      removeEventListener("StartRank", handleStartRank);
    };
  }, [gameSessionPda, leaderboardPda, playerName, program, wallet]);

  useEffect(() => {
    const handleEndGame = () => {
      endGame(program, leaderboardPda, gameSessionPda as PublicKey, wallet, finalScore);
    };

    addEventListener("EndGame", handleEndGame);
    return () => {
      removeEventListener("EndGame", handleEndGame);
    };
  }, [gameSessionPda, leaderboardPda, program, wallet]);

  const [containerStyle, setContainerStyle] = useState({});

  useEffect(() => {
    const handleResize = () => {
      const screenWidth = window.innerWidth;
      const screenHeight = window.innerHeight;
      const aspectRatio = 9 / 14; // 您的遊戲的長寬比

      let width, height;
      if (screenWidth / screenHeight > aspectRatio) {
        // 高度適配
        height = "100vh";
        width = `${(screenHeight * aspectRatio).toFixed(2)}px`;
      } else {
        // 寬度適配
        width = "100vw";
        height = `${(screenWidth / aspectRatio).toFixed(2)}px`;
      }

      setContainerStyle({
        position: "relative",
        width,
        height,
      });
    };

    handleResize();
    window.addEventListener("resize", handleResize);
    console.log('isLoaded', isLoaded)
    return () => window.removeEventListener("resize", handleResize);
  }, [isLoaded]);


  return (
    <div className="flex flex-col items-center justify-center min-h-screen bg-sky-100">
      {/* Top Bar */}
      <div className="fixed top-0 flex justify-between items-center w-full p-2 bg-[#FC7F18]">
        <div className="flex items-center justify-evenly gap-3">
          <Image src={bonktomoon} alt="bonktomoon" width={150} height={100} />
        </div>
        <WalletMultiButton
          style={{ backgroundColor: "#121214", borderRadius: "40px" }}
        />
      </div>

      <div className="relative w-auto h-auto">
        <Unity
          unityProvider={unityProvider}
          style={containerStyle}
          devicePixelRatio={devicePixelRatio}
        />
      </div>
    </div>
  );
};

export default Game;
