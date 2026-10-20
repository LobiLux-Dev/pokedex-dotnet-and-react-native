import NetInfo, { type NetInfoState } from '@react-native-community/netinfo';
import { useCallback, useEffect, useState } from 'react';
import { Platform } from 'react-native';

import { setNetworkAvailable } from '@/lib/api';

export interface NetworkStatus {
  /** True only when the device explicitly reports that it has no connection. */
  isOffline: boolean;
  /** True once a connection has been seen in this session. */
  hasBeenOnline: boolean;
  /** Re-checks the connectivity right now. */
  recheck: () => Promise<void>;
}

interface NetworkState {
  isOffline: boolean;
  hasBeenOnline: boolean;
}

function isOfflineState(state: NetInfoState): boolean {
  // `isConnected` is the reliable signal on every platform. `isInternetReachable` can be
  // stale (and is null on web) so it is left out of the blocking decision.
  return state.isConnected === false;
}

/**
 * Connectivity of the device.
 *
 * The default state is "online": if NetInfo is missing, slow or wrong, the app must still
 * open. The offline screen is only shown when the device explicitly reports no connection,
 * and only if the app has not been online yet in this session.
 */
export function useNetworkStatus(): NetworkStatus {
  const [state, setState] = useState<NetworkState>({
    isOffline: false,
    hasBeenOnline: false,
  });

  const apply = useCallback((offline: boolean) => {
    setNetworkAvailable(!offline);
    setState((previous) => ({
      isOffline: offline,
      hasBeenOnline: previous.hasBeenOnline || !offline,
    }));
  }, []);

  useEffect(() => {
    let unsubscribe: (() => void) | undefined;

    try {
      unsubscribe = NetInfo.addEventListener((netState) => {
        apply(isOfflineState(netState));
      });
    } catch {
      // Without NetInfo the app must still work: assume there is a connection. The React
      // state already defaults to online, so only the request guard needs updating.
      setNetworkAvailable(true);
    }

    return () => {
      unsubscribe?.();
    };
  }, [apply]);

  // On web NetInfo listens to navigator.connection, which does not always fire on a plain
  // network change; the browser events are the reliable fallback.
  useEffect(() => {
    if (Platform.OS !== 'web' || typeof window === 'undefined') {
      return undefined;
    }

    const handleChange = () => apply(window.navigator.onLine === false);

    window.addEventListener('online', handleChange);
    window.addEventListener('offline', handleChange);

    return () => {
      window.removeEventListener('online', handleChange);
      window.removeEventListener('offline', handleChange);
    };
  }, [apply]);

  const recheck = useCallback(async () => {
    try {
      const netState = await NetInfo.fetch();
      apply(isOfflineState(netState));
    } catch {
      apply(false);
    }
  }, [apply]);

  return { ...state, recheck };
}
