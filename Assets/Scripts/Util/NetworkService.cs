using System;
using System.Threading.Tasks;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace Util
{
    public static class NetworkService
    {
        public static string JoinCode { get; private set; }

        public static async void InitializeUnityServicesAsync()
        {
            try
            {
                await UnityServices.InitializeAsync();
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                var playerID = AuthenticationService.Instance.PlayerId;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public static async Task<RelayServerData> AllocateRelayServerAndGetJoinCode(int maxConnections, string region = null)
        {
            Allocation allocation;
            try
            {
                allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections, region);
            }
            catch (Exception e)
            {
                Debug.LogError($"Relay create allocation request failed {e.Message}");
                throw;
            }

            Debug.Log($"server: {allocation.ConnectionData[0]} {allocation.ConnectionData[1]}");
            Debug.Log($"server: {allocation.AllocationId}");

            try
            {
                JoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            }
            catch
            {
                Debug.LogError("Relay create join code request failed");
                throw;
            }

            UIManager.Instance.SetDuelHostUI(JoinCode);

            return new RelayServerData(allocation, "dtls");
        }

        public static async Task<RelayServerData> JoinRelayServerFromJoinCode(string joinCode)
        {
            JoinAllocation allocation;
            try
            {
                joinCode = joinCode.Trim().ToUpper();

                allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            }
            catch
            {
                Debug.LogError("Relay create join code request failed");
                throw;
            }

            Debug.Log($"client: {allocation.ConnectionData[0]} {allocation.ConnectionData[1]}");
            Debug.Log($"host: {allocation.HostConnectionData[0]} {allocation.HostConnectionData[1]}");
            Debug.Log($"client: {allocation.AllocationId}");

            UIManager.Instance.SetDuelClientUI();

            return new RelayServerData(allocation, "dtls");
        }
    }
}
