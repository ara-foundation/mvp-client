using MaydoneV1;
using NBitcoin;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Thirdweb;
using Thirdweb.AccountAbstraction;
using Thirdweb.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class AddWelcomePage {
    public string token;
    public int projectId;
    public int networkId;
    public string content;
}

[Serializable]
public class Sangha
{
    public string ownership_minted;
    public string ownership_max_supply;
    public string ownership; // DAO token
    public string maintainer;
    public string check;
    public string ownershipSymbol;
    public string maintainerSymbol;
    public string checkSymbol;
}

[Serializable]
public class Lungta
{
    public string aurora_id;
    public int logos_id;
    public string maydone_id;
    public string act_id;
}

[Serializable]
public class Leader
{
    public string _id;
    public int userId;
    public int nonce;
    public string username;
    public string walletAddress;
}

[Serializable]
public class Project
{
    public string _id; // "67056baf24372ef24a58420c",
    public int projectId;
    public int networkId;
    [SerializeField]
    public Sangha sangha;
    [SerializeField]
    public Lungta lungta;
    public string project_name;
    [SerializeField]
    public Leader leader;
}

public class Maydone_NewPlan : MonoBehaviour
{
    [Header("Welcome page")]
    [SerializeField] public Plan Plan;
    [SerializeField] private CardLogos WelcomeLogos;
    [Space(20)]
    [Header("Form")]
    [SerializeField] private CardLogos FormLogos;
    [Space(10)]
    [SerializeField] private Button DeployButton;
    [SerializeField] private GameObject DeployingSpinner;
    [SerializeField] private Button LastReturnButton;

    private AraDiscussion logos;
    private UserScenarioInServer userScenario;
    private bool runCoroutine = false;

    private int nextProjectId;
    private string leader;

    IEnumerator CheckProjectStatus()
    {
        while (runCoroutine)
        {
            var postTask = PostSanghaWelcome(nextProjectId);
            yield return new WaitUntil(() => postTask.IsCompleted);
            var posted = postTask.Result;

            if (posted)
            {
                runCoroutine = false;
                break;
            }
            else
            {
                Debug.Log($"Project {nextProjectId} not registered on server, waiting a second and trying again");
                yield return new WaitForSeconds(1f);
            }
        }

        Notification.Instance.Show("Project was added. Ask investors to join");
        EnableDeploy();
        Hide();
        Maydone.Instance.ResetNewPlanMode();
        Maydone.Instance.Show();
    }

    public void OnChangeProjectName(string value)
    {
        if (Plan != null)
        {
            Plan.project_name = value;
        }
    }

    public void OnChangeTechStack(string value)
    {
        if (Plan != null)
        {
            Plan.tech_stack = value;
        }
    }

    public void OnChangeCost(string value)
    {
        if (Plan != null)
        {
            Int32.TryParse(value, out Plan.cost_usd);
        }
    }

    public void OnChangeDeadline(string value)
    {
        if (Plan != null)
        {
            Int32.TryParse(value, out Plan.duration);
        }
    }

    public void OnChangeSourceCodeUrl(string value)
    {
        if (Plan != null)
        {
            Plan.source_code_url = value;
        }
    }

    public void OnChangeTestUrl(string value)
    {
        if (Plan != null)
        {
            Plan.test_url = value;
        }
    }

    public void OnChangeTokenName(string value)
    {
        if (Plan != null)
        {
            Plan.token_name = value;
        }
    }

    public void OnChangeTokenSymbol(string value)
    {
        if (Plan != null)
        {
            Plan.token_symbol = value;
        }
    }

    public void OnChangeTokenMaxSupply(string value)
    {
        if (Plan != null)
        {
            Plan.token_max_supply = value;
        }
    }

    public void OnChangeSnaghaWelcome(string value)
    {
        if (Plan != null)
        {
            Plan.sangha_welcome = value;
        }
    }

    public void Hide()
    {

        DeployButton.interactable = true;
        DeployingSpinner.SetActive(false);
        LastReturnButton.interactable = true;

        Debug.Log("Hide the new plan, reset the plan");
        Plan = new Plan();
        logos = null;
    } 

    private void DisableDeploy()
    {
        DeployButton.interactable = false;
        DeployingSpinner.SetActive(true);
    }

    private void EnableDeploy()
    {
        DeployButton.interactable = true;
        DeployingSpinner.SetActive(false);
    }

    public async Task<bool> PostSanghaWelcome(int id)
    {
        string url = NetworkParams.AraActUrl + $"/maydone/plan/welcome";

        var data = new AddWelcomePage()
        {
            token = AraAuth.Instance.UserParams.token,
            projectId = id,
            networkId = NetworkParams.networkId,
            content = Plan.sangha_welcome,
        };
        var body = JsonConvert.SerializeObject(data);

        Tuple<long, string> res;
        try
        {
            res = await WebClient.Post(url, body);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            return false;
        }
        if (res.Item1 != 200)
        {
            return false;
        }

        return true;
    }

    public async void OnDeploy()
    {
        
        DisableDeploy();
        var lastProjectId = await ThirdwebContract.Read<BigInteger>(AraAuth.Instance.ProjectContract, "lastProjectId", new object[] { });
        Debug.Log($"last project id {lastProjectId}");
        nextProjectId = (int)lastProjectId + 1;
        this.leader = await AraAuth.Instance.GetAddress();

        var balance = await AraAuth.Instance.GetBalance();
        if (balance <= 0)
        {
            Notification.Instance.Show($"No native token to pay for transaction. Top up your address {this.leader}");
            EnableDeploy();
            return;
        }
        Notification.Instance.Show($"Balance: {balance}");

        var error = Plan.Validate();
        if (!string.IsNullOrEmpty(error))
        {
            Notification.Instance.Show("Error: " + error);
            EnableDeploy();
            return;
        }
        if (!AraAuth.Instance.IsLoggedIn(AraAuth.Instance.UserParams))
        {
            AraAuth.Instance.RequireLogin();
            EnableDeploy();
            return;
        }


        var tokenArgs = new object[] { Web3.Convert.ToWei(Plan.token_max_supply), Plan.token_symbol, Plan.token_name };
        var projectArgs = new object[]
        {
            false, // if the project is cancelled
            //
            // Data
            //    
            Plan.project_name, // project name
            JsonConvert.SerializeObject(logos),   // the hardcoded logos snapshotted.
            JsonConvert.SerializeObject(userScenario),  // the hardcoded aurora snapshotted.
            Plan.tech_stack,
            Web3.Convert.ToWei(Plan.cost_usd),
            Plan.duration * 86400, // An ACT stage duration
            Plan.source_code_url,
            Plan.test_url,
            // Dynamic data set by Maydone
            0 // Start Time An ACT stage start time
        };

        for (var i = 0; i < tokenArgs.Length; i++)
        {
            Debug.Log($"{i+1}: {tokenArgs[i]}");
        }
        for (var i = 0; i < projectArgs.Length; i++)
        {
            Debug.Log($"{i + 1}: {projectArgs[i]}");
        }

        var token = new Token()
        {
            MaxSupply = Web3.Convert.ToWei(Plan.token_max_supply),
            Symbol = Plan.token_symbol,
            Name = Plan.token_name,
        };
        var projectData = new MaydoneV1.Project()
        {
            Active = false,
            Name = Plan.project_name,
            Logos = JsonConvert.SerializeObject(logos),
            Aurora = JsonConvert.SerializeObject(userScenario),
            TechStack = Plan.tech_stack,
            CostUsd = Web3.Convert.ToWei(Plan.cost_usd),
            Duration = Web3.Convert.ToWei(Plan.duration * 86400),
            SourceCodeUrl = Plan.source_code_url,
            TestUrl = Plan.test_url,
            StartTime = BigInteger.Zero
        };

        await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(2f));

        // Executing the transfer
        try
        {
            ThirdwebTransaction transaction = await ThirdwebContract.Prepare(AraAuth.Instance.Wallet, AraAuth.Instance.MaydoneContract, "launch", BigInteger.Zero, token, projectData);
            var limit = await Maydone_NewPlan.EstimateGasLimit(transaction);
            if (limit <= 0)
            {
                throw new Exception("failed to estimate a gas limit");
            }

            var receipt = await Maydone_NewPlan.Send(transaction);
        
            Notification.Instance.Show($"Transaction: {receipt} deployed, waiting for a result");
            Notification.Instance.Show($"Don't close the client please!");
        } catch (Exception ex)
        {
            Debug.LogError(ex);
            Notification.Instance.Show($"Failed to send transaction: {ex}");
            EnableDeploy();
            return;
        }

        runCoroutine = true;
        StartCoroutine(CheckProjectStatus());
    }

    public static async Task<ThirdwebTransaction> Prepare(ThirdwebTransaction transaction)
    {
        if (transaction.Input.To == null)
        {
            throw new InvalidOperationException("Transaction recipient (to) must be provided");
        }

        if (transaction.Input.GasPrice != null && (transaction.Input.MaxFeePerGas != null || transaction.Input.MaxPriorityFeePerGas != null))
        {
            throw new InvalidOperationException("Transaction GasPrice and MaxFeePerGas/MaxPriorityFeePerGas cannot be set at the same time");
        }

        ThirdwebTransactionInput input = transaction.Input;
        if ((object)input.Nonce == null)
        {
            ThirdwebTransactionInput thirdwebTransactionInput = input;
            thirdwebTransactionInput.Nonce = new HexBigInteger(await ThirdwebTransaction.GetNonce(transaction).ConfigureAwait(continueOnCapturedContext: false));
        }

        input = transaction.Input;
        if ((object)input.Value == null)
        {
            input.Value = new HexBigInteger(0);
        }

        input = transaction.Input;
        if (input.Data == null)
        {
            input.Data = "0x";
        }

        input = transaction.Input;
        if ((object)input.Gas == null)
        {
            ThirdwebTransactionInput thirdwebTransactionInput = input;
            thirdwebTransactionInput.Gas = new HexBigInteger(await EstimateGasLimit(transaction).ConfigureAwait(continueOnCapturedContext: false));
        }

        if (Thirdweb.Utils.IsEip1559Supported(NetworkParams.networkId.ToString()))
        {
            if (transaction.Input.GasPrice == null)
            {
                (BigInteger, BigInteger) obj = await ThirdwebTransaction.EstimateGasFees(transaction).ConfigureAwait(continueOnCapturedContext: false);
                BigInteger item = obj.Item1;
                BigInteger item2 = obj.Item2;
                input = transaction.Input;
                if ((object)input.MaxFeePerGas == null)
                {
                    input.MaxFeePerGas = new HexBigInteger(item);
                }

                input = transaction.Input;
                if ((object)input.MaxPriorityFeePerGas == null)
                {
                    input.MaxPriorityFeePerGas = new HexBigInteger(item2);
                }
            }
        }
        else if (transaction.Input.MaxFeePerGas == null && transaction.Input.MaxPriorityFeePerGas == null)
        {
            input = transaction.Input;
            if ((object)input.GasPrice == null)
            {
                ThirdwebTransactionInput thirdwebTransactionInput = input;
                thirdwebTransactionInput.GasPrice = new HexBigInteger(await ThirdwebTransaction.EstimateGasPrice(transaction).ConfigureAwait(continueOnCapturedContext: false));
            }
        }

        return transaction;
    }


    public static async Task<BigInteger> EstimateGasLimit(ThirdwebTransaction transaction)
    {
        ThirdwebRPC rpcInstance = ThirdwebRPC.GetRpcInstance(AraAuth.Instance.Wallet.Client, NetworkParams.networkId);
        return new HexBigInteger(await rpcInstance.SendRequestAsync<string>("eth_estimateGas", new object[1] { transaction.Input }).ConfigureAwait(continueOnCapturedContext: false)).Value * 10 / 7;
    }

    public static async Task<string> Send(ThirdwebTransaction transaction)
    {
        transaction = await Prepare(transaction).ConfigureAwait(continueOnCapturedContext: false);
        ThirdwebRPC rpc = ThirdwebRPC.GetRpcInstance(AraAuth.Instance.Wallet.Client, NetworkParams.networkId);
        string result;
            switch (AraAuth.Instance.Wallet.AccountType)
            {
                case ThirdwebAccountType.PrivateKeyAccount:
                    {
                        string text2 = await ThirdwebTransaction.Sign(transaction);
                        result = await rpc.SendRequestAsync<string>("eth_sendRawTransaction", new object[1] { text2 }).ConfigureAwait(continueOnCapturedContext: false);
                        break;
                    }
                case ThirdwebAccountType.SmartAccount:
                case ThirdwebAccountType.ExternalAccount:
                {
                    result = await AraAuth.Instance.Wallet.SendTransaction(transaction.Input).ConfigureAwait(continueOnCapturedContext: false);
                    break;
                }
                default:
                    throw new NotImplementedException("Account type not supported");
            }

        return result;
    }

    public void Show(AraDiscussion logos, UserScenarioInServer userScenario)
    {
        if (runCoroutine)
        {
            runCoroutine = false;
            StopAllCoroutines();
        }
        // First, show the Warning
        // Then, show the Welcome:
        WelcomeLogos.Show(logos);
        FormLogos.Show(logos);
        DeployingSpinner.SetActive(false);

        this.logos = logos;
        this.userScenario = userScenario;

        Debug.Log("Attach leader...");

        Plan = new Plan();
        if (userScenario != null)
        {
            Plan.user_scenario_id = userScenario._id;
        }
        if (logos != null)
        {
            Plan.logos_id = logos.id;
        }
        if (AraAuth.Instance.IsLoggedIn(AraAuth.Instance.UserParams))
        {
            AttachLeader(true);
        } else
        {
            AraAuth.Instance.OnStatusChange += AttachLeader;
        }
    }

    private void AttachLeader(bool loggedIn)
    {
        if (!loggedIn)
        {
            return;
        }
        Debug.Log($"Attaching {AraAuth.Instance.UserParams.loginParams.user_id} and {AraAuth.Instance.UserParams.loginParams.username}");
        AraAuth.Instance.OnStatusChange -= AttachLeader;
        Plan.leader_username = AraAuth.Instance.UserParams.loginParams.username;
        Plan.leader_user_id = AraAuth.Instance.UserParams.loginParams.user_id;
    }
}
