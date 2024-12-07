import "@/styles/globals.css";
import type { AppProps } from "next/app";
import dynamic from 'next/dynamic';

const AppWalletProvider = dynamic(() => import('../components/AppWalletProvider'), { ssr: false });

export default function App({ Component, pageProps }: AppProps) {
  return (
    <>
      <AppWalletProvider>
        <Component {...pageProps} />
      </AppWalletProvider>
    </>
  );
}
