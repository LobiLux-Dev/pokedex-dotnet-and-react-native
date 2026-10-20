import NetInfo, { type NetInfoState } from '@react-native-community/netinfo';
import { useCallback, useEffect, useState } from 'react';

import { setNetworkAvailable } from '@/lib/api';

export interface NetworkStatus {
  /** False until the first connectivity reading is available. */
  isReady: boolean;
  isOffline: boolean;
  /** True once the device has been online at least once in this session. */
  hasBeenOnline: boolean;
  /** Re-checks the connectivity right now. */
  recheck: () => Promise<void>;
}

interface NetworkState {
  isReady: boolean;
  isOffline: boolean;
  hasBeenOnline: boolean;
}

function isOfflineState(state: NetInfoState): boolean {
  // `isConnected` is the reliable signal on every platform. `isInternetReachable` can be
  // stale (and is null on web) so it is left out of the blocking decision.
  return state.isConnected === false;
}

function toNetworkState(state: NetInfoState, previous: NetworkState): NetworkState {
  const isOffline = isOfflineState(state);

  return {
    isReady: true,
    isOffline,
    hasBeenOnline: previous.hasBeenOnline || !isOffline,
  };
}

/**
 * Connectivity of the device. When there is no network the app shows a blocking
 * error screen instead of firing requests that are going to fail.
 */
export function useNetworkStatus(): NetworkStatus {
  const [state, setState] = useState<NetworkState>({
    isReady: false,
    isOffline: false,
    hasBeenOnline: false,
  });

  useEffect(() => {
    const unsubscribe = NetInfo.addEventListener((netState) => {
      setNetworkAvailable(!isOfflineState(netState));
      setState((previous) => toNetworkState(netState, previous));
    });

    return unsubscribe;
  }, []);

  const recheck = useCallback(async () => {
    const netState = await NetInfo.fetch();
    setNetworkAvailable(!isOfflineState(netState));
    setState((previous) => toNetworkState(netState, previous));
  }, []);

  return { ...state, recheck };
}
