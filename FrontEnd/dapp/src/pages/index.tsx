"use client";
import { useEffect, useState } from "react";
import { Unity, useUnityContext } from "react-unity-webgl";
import Image from "next/image";
import fansLoverLogo from "/public/fansLoverLogo.jpg";
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
      <div className="absolute top-4 left-0 right-0 flex justify-between items-center p-2">
        <div className="flex items-center justify-evenly gap-3">
          <Image
            src={fansLoverLogo}
            alt="fans_lover_logo"
            width={50}
            height={50}
            className="rounded-full"
          />
          <div className="flex flex-col items-center justify-center gap-1">
            <span className="font-bold">FansLover </span>
          </div>
        </div>
        <div className="border hover:border-slate-900 rounded">
          <WalletMultiButton />
        </div>
      </div>

      <div className="relative w-auto h-auto bg-white rounded-lg p-1">
        <Unity
          unityProvider={unityProvider}
          style={{
            height: "calc(100vh - 11rem)",
            width: `calc((100vh - 11rem) * (9 / 16))`,
            borderRadius: "15px",
          }}
          devicePixelRatio={devicePixelRatio}
        />
      </div>
      {/* Bottom Nav */}
      <div className="absolute bottom-7 left-0 right-0 px-2 ">
        <div className="flex justify-center items-center gap-1">
          <button
            className="rounded-3xl w-1/2 p-1  sm:p-2 md:p-3 flex items-center justify-center"
          >
            <span className="font-light">test</span>
          </button>
        </div>
      </div>
      <audio src="/bgm.mp3" muted={false} autoPlay loop />
    </div>
  );
};

export default Game;
