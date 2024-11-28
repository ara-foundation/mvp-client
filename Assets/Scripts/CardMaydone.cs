using NBitcoin;
using Nethereum.Web3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Thirdweb;
using Thirdweb.Unity;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CardMaydone: MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Title;

    [SerializeField] private CardLogos logosCard;

    [Header("Collective")]
    [SerializeField] private TextMeshProUGUI DisplayName;
    [SerializeField] private TextMeshProUGUI UserName;
    [Space(10)]
    [Header("Project")]
    [SerializeField] private TextMeshProUGUI ProjectName;
    [SerializeField] private TextMeshProUGUI Welcome;
    [SerializeField] private TMP_InputField InvestInput;
    [SerializeField] private Slider InvestSlider;
    [SerializeField] private Button InvestButton;
    [SerializeField] private TextMeshProUGUI MintedPercentageText;
    [SerializeField] private TextMeshProUGUI OwningTokenText;
    private decimal costUsd = 0;
    private decimal ownershipMinted = 0;
    private decimal ownershipSupply = 0;
    private decimal leftPercentage = 0;

    private PlanWithProject planWithProject;
    private AraDiscussion logos;

    public async void Show(PlanWithProject planWithProject)
    {
        this.planWithProject = planWithProject;
        Title.text = $"Join to {planWithProject.project_v1[0].sangha.ownershipSymbol} Sangha";
        DisplayName.text = planWithProject.project_v1[0].leader.walletAddress;
        UserName.text = planWithProject.project_v1[0].leader.username;
        ProjectName.text = planWithProject.project_v1[0].project_name;

        // Fetch the logos
        logosCard.gameObject.SetActive(false);
        logos = await Logos.Instance.FetchIdea(planWithProject.project_v1[0].lungta.logos_id);
        if (logos == null)
        {
            Notification.Show("Error: failed to get logos idea from the server");
            return;
        }
        logosCard.gameObject.SetActive(true);
        logosCard.Show(logos);

        costUsd = Web3.Convert.FromWei(BigInteger.Parse(planWithProject.cost_usd));
        ownershipMinted = Web3.Convert.FromWei(BigInteger.Parse(planWithProject.project_v1[0].sangha.ownership_minted));
        ownershipSupply = Web3.Convert.FromWei(BigInteger.Parse(planWithProject.project_v1[0].sangha.ownership_max_supply));
        leftPercentage = 100 - (ownershipMinted / (ownershipSupply / 100));

        MintedPercentageText.text = $"{leftPercentage}% left";
        InvestSlider.maxValue = (float)((costUsd / 100) * leftPercentage);
        OwningTokenText.text = $"0 {planWithProject.project_v1[0].sangha.ownershipSymbol}";
    }

    public void OnValueChange (string value)
    {
        var newValue = float.Parse(value);
        if (newValue > InvestSlider.maxValue)
        {
            InvestInput.text = InvestSlider.maxValue.ToString();
        }
        InvestSlider.value = newValue;
    }

    // From Slider
    public void OnValueChange()
    {
        InvestInput.text = InvestSlider.value.ToString();

        if (InvestSlider.value == 0)
        {
            OwningTokenText.text = $"0 {planWithProject.project_v1[0].sangha.ownershipSymbol}";
            return;
        }
        var percentage = InvestSlider.maxValue / 1;
        var potentialPercentage = (decimal)(InvestSlider.value / (percentage));
        var ownableOwnership = ownershipSupply * (potentialPercentage/100);

        OwningTokenText.text = $"{ownableOwnership.ToString("0.0000")} {planWithProject.project_v1[0].sangha.ownershipSymbol}";
    }
    private void DisableAction()
    {
        InvestSlider.interactable = false;
        InvestInput.interactable = false;
        InvestButton.interactable = false;
    }

    private void EnableAction()
    {
        InvestSlider.interactable = true;
        InvestInput.interactable = true;
        InvestButton.interactable = true;
    }

    public async void OnInvest()
    {
        if (InvestSlider.value == 0)
        {
            Notification.Show("Error: no investment amount");
            return;
        }
        if (!AraAuth.Instance.IsLoggedIn(AraAuth.Instance.UserParams))
        {
            AraAuth.Instance.RequireLogin();
            return;
        }

        var myAddress = await AraAuth.Instance.GetAddress();
        var stableAddress = NetworkParams.StableCollateralAddress(NetworkParams.networkId);
        var treasuryAddress = NetworkParams.TreasuryAddress(NetworkParams.networkId);
        var amount = Web3.Convert.ToWei(InvestSlider.value, NetworkParams.StableCollateralDecimals);
        Debug.Log($"This amount is taken as the investment: {amount} Stable coin");
        var amountInWei = Web3.Convert.ToWei(InvestSlider.value);
        Debug.Log($"Amount in wei: {amountInWei}");
        var StableCoin = await ThirdwebManager.Instance.GetContract(stableAddress, NetworkParams.networkId, NetworkParams.Erc20Abi);

        DisableAction();

        var allowance = await ThirdwebContract.Read<BigInteger>(StableCoin, "allowance", myAddress, treasuryAddress);

        if (allowance < amount)
        {
            Notification.Show($"Getting a permission to spend {InvestSlider.value} from your wallet.");
            var approveAmount = Web3.Convert.ToWei(100000000, NetworkParams.StableCollateralDecimals);

            try
            {
                ThirdwebTransaction transaction = await ThirdwebContract.Prepare(AraAuth.Instance.Wallet, StableCoin, "approve", BigInteger.Zero, treasuryAddress, approveAmount);
                var limit = await Maydone_NewPlan.EstimateGasLimit(transaction);
                if (limit <= 0)
                {
                    throw new Exception("failed to estimate a gas limit");
                }

                var receipt = await Maydone_NewPlan.Send(transaction);
                Notification.Show($"Approval tx: ${receipt}; Now minting your ownership tokens...");
            } catch (Exception ex)
            {
                Notification.Show(ex.Message);
                EnableAction();
                return;
            }
        }

        var Treasury = await ThirdwebManager.Instance.GetContract(treasuryAddress, NetworkParams.networkId, NetworkParams.TreasuryAbi);

        Debug.Log($"mintByUsd(uint256 projectId_ {planWithProject.project_v1[0].projectId}, address to_ {myAddress}, uint256 usdAmount_ {amountInWei}, address collateral_ {stableAddress})");
          
        // Executing the transfer
        try
        {
            ThirdwebTransaction transaction = await ThirdwebContract.Prepare(AraAuth.Instance.Wallet, Treasury, "mintByUsd", BigInteger.Zero, planWithProject.project_v1[0].projectId, await AraAuth.Instance.GetAddress(), amountInWei, stableAddress);
            var limit = await Maydone_NewPlan.EstimateGasLimit(transaction);
            if (limit <= 0)
            {
                throw new Exception("failed to estimate a gas limit");
            }

            var receipt = await Maydone_NewPlan.Send(transaction);

            Notification.Show($"Transaction: {receipt} deployed! You received your tokens.");
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            Notification.Show($"Failed to send transaction: {ex}");
        }
        EnableAction();
        InvestSlider.value = 0;
    }
}
