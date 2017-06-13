using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Breeze.TumbleBit.Models;
using Flurl;
using Flurl.Http;
using Flurl.Http.Configuration;
using NBitcoin;
using NBitcoin.JsonConverters;
using Newtonsoft.Json;
using NTumbleBit;
using NTumbleBit.ClassicTumbler;
using NTumbleBit.JsonConverters;
using NTumbleBit.PuzzlePromise;
using NTumbleBit.PuzzleSolver;
using PuzzlePromise = NTumbleBit.PuzzlePromise;
using PuzzleSolver = NTumbleBit.PuzzleSolver;

namespace Breeze.TumbleBit.Client
{
    public class TumblerService : ITumblerService
    {
        private readonly string serverAddress;

        public TumblerService(Uri serverAddress)
        {
            this.serverAddress = serverAddress.ToString();
            FlurlHttp.Configure(c => {
                c.JsonSerializer = new NewtonsoftJsonSerializer(new JsonSerializerSettings
                {
                    Converters = new List<JsonConverter> { new NetworkJsonConverter(), new RsaKeyJsonConverter(), new UInt256JsonConverter() }

                });
            });
        }

        /// <inheritdoc />
        public async Task<ClassicTumblerParameters> GetClassicTumblerParametersAsync()
        {
            ClassicTumblerParameters result = await this.serverAddress.AppendPathSegment("/api/v1/tumblers/0/parameters").GetJsonAsync<ClassicTumblerParameters>();
            return result;
        }

        /// <inheritdoc />
        public async Task<UnsignedVoucherInformation> AskUnsignedVoucherAsync()
        {
            UnsignedVoucherInformation result = await this.serverAddress.AppendPathSegment("api/v1/tumblers/0/vouchers/").GetJsonAsync<UnsignedVoucherInformation>();
            return result;
        }

        /// <inheritdoc />
        public async Task<PuzzleSolution> SignVoucherAsync(SignVoucherRequest request)
        {
            PuzzleSolution result = await this.serverAddress.AppendPathSegment("api/v1/tumblers/0/clientchannels/confirm").PostJsonAsync(request).ReceiveJson<PuzzleSolution>();
            return result;
        }

        /// <inheritdoc />
        public async Task<ScriptCoin> OpenChannelAsync(OpenChannelRequest request)
        {
            ScriptCoin result = await this.serverAddress.AppendPathSegment("api/v1/tumblers/0/channels/").PostJsonAsync(request).ReceiveJson<ScriptCoin>();
            return result;
        }

        /// <inheritdoc />
        public async Task<TumblerEscrowKeyResponse> RequestTumblerEscrowKeyAsync(int cycleStart)
        {
            TumblerEscrowKeyResponse result = await this.serverAddress.AppendPathSegment("api/v1/tumblers/0/clientchannels/").PostJsonAsync(cycleStart).ReceiveJson<TumblerEscrowKeyResponse>();
            return result;
        }

        /// <inheritdoc />
        public async Task<ServerCommitmentsProof> CheckRevelationAsync(int cycleId, string channelId, PuzzlePromise.ClientRevelation revelation)
        {
            ServerCommitmentsProof result = await this.serverAddress.AppendPathSegment($"api/v1/tumblers/0/channels/{cycleId}/{channelId}/checkrevelation").PostJsonAsync(revelation).ReceiveJson<ServerCommitmentsProof>();
            return result;
        }

        /// <inheritdoc />
        public async Task<SolutionKey[]> CheckRevelationAsync(int cycleId, string channelId, PuzzleSolver.ClientRevelation revelation)
        {
            SolutionKey[] result = await this.serverAddress.AppendPathSegment($"api/v1/tumblers/0/clientschannels/{cycleId}/{channelId}/checkrevelation").PostJsonAsync(revelation).ReceiveJson<SolutionKey[]>();
            return result;
        }

        /// <inheritdoc />
        public async Task<PuzzlePromise.ServerCommitment[]> SignHashesAsync(int cycleId, string channelId, SignaturesRequest sigReq)
        {
            PuzzlePromise.ServerCommitment[] result = await this.serverAddress.AppendPathSegment($"api/v1/tumblers/0/channels/{cycleId}/{channelId}/signhashes").PostJsonAsync(sigReq).ReceiveJson<PuzzlePromise.ServerCommitment[]>();
            return result;
        }

        /// <inheritdoc />
        public async Task<OfferInformation> CheckBlindFactorsAsync(int cycleId, string channelId, BlindFactor[] blindFactors)
        {
            OfferInformation result = await this.serverAddress.AppendPathSegment($"api/v1/tumblers/0/clientschannels/{cycleId}/{channelId}/checkblindfactors").PostJsonAsync(blindFactors).ReceiveJson<OfferInformation>();
            return result;
        }

        /// <inheritdoc />
        public async Task<PuzzleSolver.ServerCommitment[]> SolvePuzzlesAsync(int cycleId, string channelId, PuzzleValue[] puzzles)
        {
            PuzzleSolver.ServerCommitment[] result = await this.serverAddress.AppendPathSegment($"api/v1/tumblers/0/clientchannels/{cycleId}/{channelId}/solvepuzzles").PostJsonAsync(puzzles).ReceiveJson<PuzzleSolver.ServerCommitment[]>();
            return result;
        }

        /// <inheritdoc />
        public async Task<SolutionKey[]> FulfillOfferAsync(int cycleId, string channelId, TransactionSignature clientSignature)
        {
            SolutionKey[] result = await this.serverAddress.AppendPathSegment($"api/v1/tumblers/0/clientchannels/{cycleId}/{channelId}/offer").PostJsonAsync(clientSignature).ReceiveJson<SolutionKey[]>();
            return result;
        }

        /// <inheritdoc />
        public async Task GiveEscapeKeyAsync(int cycleId, string channelId, TransactionSignature signature)
        {
            await this.serverAddress.AppendPathSegment($"api/v1/tumblers/0/clientchannels/{cycleId}/{channelId}/escape").PostJsonAsync(signature);            
        }
    }
}