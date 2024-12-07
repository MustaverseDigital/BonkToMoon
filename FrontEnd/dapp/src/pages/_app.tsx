import "@/styles/globals.css";
import type { AppProps } from "next/app";
import AppWalletProvider from "../components/AppWalletProvider";

export default function App({ Component, pageProps }: AppProps) {
  return (
    <>
      <AppWalletProvider>
        <Component {...pageProps} />
      </AppWalletProvider>
    </>
  );
}
