"use client";
import { useEffect, useState } from "react";
import { Unity, useUnityContext } from "react-unity-webgl";
import Image from "next/image";
import bonktomoon from "/public/bonktomoon.png";
import { WalletMultiButton } from "@solana/wallet-adapter-react-ui";
import { useWallet } from "@solana/wallet-adapter-react"


const Game = () => {
  const { publicKey } = useWallet();
  console.log(publicKey);

  const { unityProvider } = useUnityContext({
    loaderUrl: "/Build/GameBuild.loader.js",
    dataUrl: "/Build/GameBuild.data",
    frameworkUrl: "/Build/GameBuild.framework.js",
    codeUrl: "/Build/GameBuild.wasm",
  });

  // We'll use a state to store the device pixel ratio.
  const [devicePixelRatio, setDevicePixelRatio] = useState(
    typeof window !== "undefined" ? window.devicePixelRatio : 1
  );

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
      <div className="flex justify-between items-center w-full p-2 bg-[#FC7F18]" >
        <div className="flex items-center justify-evenly gap-3">
          <Image
            src={bonktomoon}
            alt="bonktomoon"
            width={150}
            height={100}
          />
        </div>
        <WalletMultiButton style={{ backgroundColor: '#121214', borderRadius: '40px' }} />
      </div>

      <div className="relative w-full h-auto">
        <Unity
          unityProvider={unityProvider}
          style={{
            height: "calc(100vh - 64px)",
            width: '100%',
          }}
          devicePixelRatio={devicePixelRatio}
        />
      </div>
      <audio src="/bgm.mp3" muted={false} autoPlay loop />
    </div>
  );
};

export default Game;
