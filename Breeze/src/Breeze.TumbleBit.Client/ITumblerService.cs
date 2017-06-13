using System.Threading.Tasks;
using Breeze.TumbleBit.Models;
using NBitcoin;
using NTumbleBit;
using NTumbleBit.ClassicTumbler;
using NTumbleBit.PuzzlePromise;
using NTumbleBit.PuzzleSolver;
using PuzzlePromise = NTumbleBit.PuzzlePromise;
using PuzzleSolver = NTumbleBit.PuzzleSolver;

namespace Breeze.TumbleBit.Client
{
    /// <summary>
    /// The tumbler service communicating with the tumbler server.
    /// </summary>
    public interface ITumblerService
    {
        /// <summary>
        /// Gets the tumbler's parameters.
        /// </summary>
        /// <returns></returns>
        Task<ClassicTumblerParameters> GetClassicTumblerParametersAsync();

        Task<UnsignedVoucherInformation> AskUnsignedVoucherAsync();

        Task<PuzzleSolution> SignVoucherAsync(SignVoucherRequest signVoucherRequest);

        Task<ScriptCoin> OpenChannelAsync(OpenChannelRequest request);

        Task<TumblerEscrowKeyResponse> RequestTumblerEscrowKeyAsync(int cycleStart);

        Task<ServerCommitmentsProof> CheckRevelationAsync(int cycleId, string channelId, PuzzlePromise.ClientRevelation revelation);

        Task<SolutionKey[]> CheckRevelationAsync(int cycleId, string channelId, PuzzleSolver.ClientRevelation revelation);

        Task<PuzzlePromise.ServerCommitment[]> SignHashesAsync(int cycleId, string channelId, SignaturesRequest sigReq);
        
        Task<OfferInformation> CheckBlindFactorsAsync(int cycleId, string channelId, BlindFactor[] blindFactors);

        Task<PuzzleSolver.ServerCommitment[]> SolvePuzzlesAsync(int cycleId, string channelId, PuzzleValue[] puzzles);

        Task<SolutionKey[]> FulfillOfferAsync(int cycleId, string channelId, TransactionSignature clientSignature);

        Task GiveEscapeKeyAsync(int cycleId, string channelId, TransactionSignature signature);
    }
}
