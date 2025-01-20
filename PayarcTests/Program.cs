namespace TestPayarcSDK;
using Payarc;

public static class Program
{
    public static Payarc Payarc;
    public static async Task  Main(string[] args)
    {
        PayarcConfiguration.ApiKey = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJhdWQiOiIyNiIsImp0aSI6IjZkNTA1YTA2YjdlNWMwYTk5OGU4MWQ2YjJjYmJmMWFjNzFkMGFlNWU0MmE2MjIxMzk5NGZhYzE4MzI1ZWVlNDhhOGQ0MDRkZTVjNDc5NTYyIiwiaWF0IjoxNjU5MDIwNjUyLCJuYmYiOjE2NTkwMjA2NTIsImV4cCI6MTgxNjY4OTg1Miwic3ViIjoiMTU0MTQiLCJzY29wZXMiOiIqIn0.KDaegyhna7eFAQRQfIuYv-nVFPI2G7iMyCR4fyEFwtirXOxAky7Sie0Oc8HJXiuQ9k7UaNhGF3akFo_8vipTBfQD6L-_wbr8Nj3vf_EuX2Yjz35e5HBBwaJ3b5vGXJGsu_UlpgIntLYPW6DRFSWAdzTtv0t6uyW_98oVvhm-Yxf6stj-UR3mHA18tjP1ISti53Oc2BrIKH_s58eFFdyzj6q1Q63r05uAQ9XC96Gl35ZCnaGEzcWgviTxCrKVfMuRKCzFKB-rJPlRM1lfzGcz-5wvWuqp0jWgsS383I7Pn1uXRWTxasHM93-ioa7TCWlOtyvmrs1_HIG8x9c2QgUoPXGhKzCA8pYiYyeWfguPX250P03B3hks3hZSMu6L9f1xbCBBFH484oMdxYn-CUAOQhysFN7b9-O6PW0Ge2XhsCA3rs3c9vEewTlPggoa8WHr4tTOS1GdCFxneFPzHjsbB3C-ig7r7Qq6594nw-Bb_l2ONsVmsQ19GTU08zwq4hKtdKyPI879pvh9f-IDqJliHAc1qXga5pqX_Cj4pkcTihltdQ3Q0KyKp-Wi3O0Xi6afw8EXcrVLlDnVtyic8sbBpl8Gh8EVOL1zq8D4qwrWH-oZSITmZaBqRiTz-UaKmRyMVg0yVxAfGfqjXtNYkINJEQxwS2X-Id_Mr0sUb72LmyE";
        // PayarcConfiguration.ApiKey = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJhdWQiOiIyMDY5NSIsImp0aSI6IjRhZDlkNGJiNmJlMzgxYjQwODNmY2ZhOGNhNTc5YmRjZDIxOTViMjVjZTY5NGM3OTVmMTQ2OGY4MzY2NGJjYTBhZmIxNjZmZDM2NTYxNDMyIiwiaWF0IjoxNzMzMzI2NDcyLCJuYmYiOjE3MzMzMjY0NzIsImV4cCI6MTg5MTAwNjQ3Miwic3ViIjoiNDUxMTIiLCJzY29wZXMiOiIqIn0.PlCqu3fZ0AJYUI5A-OwFBjmlch1E_i6fyvgbywTCBIdxfj0m3Bc0wEv--W2wYbvpT2b_7juQ2NgxPer80QMQXLbp8FZiN0F1lpJkcfAa5MPnC21C4d9p667M3Q8yMfNnZ150jZAxkDApl44KNZDCABPsoVgszfIYUByunE72Z5DgXGKky6iTccynn7-3AbVToxbEtpbFewY7F_CsstQZzxdDT-cmpyicof7K1MRTz5tePUYHjIqBVhZCdl0Wnig_Voxd4-4iJRP7u4ipj89D-XNq7ZD-PsJYwSLd38i0us2jAdFVNiTWlcgeF1YvXZwv1z2g-ljd9lZZHC6VR1On_WynYRprigD4a_C7-8Zx298Q696uiRpbz2a7HZG3FnNQ9QfybgxRr5O4YA32EeOTViWUkTXiWoe4UtXSRomweu4DT0I_TxvGFxMbR_T4AfIPg_xJqRZTSo_PZ7ZFCVv-YCuxp6lRkHZIgadICL6I7XW8R5iQP4HYjiB2ocPxXtKkjCLrKVkQVXRYa2va9QF3aRk_f0eRDKbm70Aw2ge5bwZvf8vOfvPDTpVKCdr1tMWElpceGNWBWQ8Q75ArAq5_Hsiu_SMGd6Oc1ej4KQ2lRxa_170PT1awYTcpoEsAkvzTKgPQZFbs-T9fX34HinjGETn3l-PO-INAtGauoOAagEI";
        PayarcConfiguration.AgentKey = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJhdWQiOiIxNDEzMSIsImp0aSI6IjhmMGI3Mjk1MWU4ZjFmNDVkZTBkMGNiNDUzNzQ0NmM2ZDNiZjQ2YmU1MzUzM2Y0ZTdjM2Y1NTgxYjQ4NmQ5NzgxMzBjZmY5Yjg5NzQ0MTJjIiwiaWF0IjoxNzIwNjA0MDI0LjAwNzM1OCwibmJmIjoxNzIwNjA0MDI0LjAwNzM2LCJleHAiOjE3NTIxNDAwMjMuOTkwOTI3LCJzdWIiOiIyNjgyNCIsInNjb3BlcyI6W119.MJd00DURXiEND3skh94tTm_KryrDpMzaamhAN_7H_Oo80OZicf7G3Y0fJbEir2RUeiHDup5kRY7R3oZ6Y4BBkFHe-DGSZtwca9wgmrzBpNFaDt2l0dBUmvBUAK80V_OEajHb911VInjWyZ5UgUhm0gMFSj5dqh8eZrmmsgS8J4vwtJd2ONTR-z2HUqdGYGc4PRh0q-69KEnESddETlE5O_NxY8jy-wwzv5TbjAasv8ddjFBuKXw_s8kgxlG1r9coLev4ECZqC2fDcC1dlTVZ1tBpMPel6wMjevuG6_GF5ZDaV44_LoOXC3cLfu3CkFIYv3RKR7sALrXjrVHGdmmeWXaTqSJ8_APtYyMXuPkWuszhZX04wgJ0cZv8haijd0gBk8esw9xlde2Iy5ZzrJFtVaBE3i_eI5q9Ptfsh-k0EUzwri4AaNX4S0jczvPDbhf4X59jiKVxBfG2xRRCVGZqXNYySHM7t7tm1e59GGkLAX2Edrsi8GglqRX4b-DFteCTNjPGCQCS7KtCEvJlZ5w2m9EddUnUvjv5j9l9llQ5HLhjLvS03X2OnJfRvTaOro-JGH4fo1F-aRK9zJZhDs2yGckIJZmk4dQQz4kwZj-A0_I1aJDKA0sxfDyXjw2St-B3rn7hPwJcRnJEVg_xCiqvUN-bpfEx7b9NgRntvOwf4ic";
        PayarcConfiguration.BaseUrl = "http://localapi6.payarc.net";
        
        Payarc = new Payarc();
        
        // await CreateChargeExample(); 
        await CreateChargeByCardIdExample();
        // await GetChargeById();
    }
    

    private static async Task CreateChargeExample()
    {
        var options = new ChargeCreateOptions
        {
            Amount = 185,
            Source = new CardCreateNestedOptions
            {
                CardNumber = "4012000098765439",
                ExpMonth = "03",
                ExpYear = "2025",
                CountyCode = "USA"
            },
            Currency = "usd"
        };
        var charge = await Payarc.Charges.Create(options);
        Console.WriteLine("Charge Data");
        Console.WriteLine(charge);
        Console.WriteLine("Raw Data");
        Console.WriteLine(charge?.RawData);
    }

    private static async Task CreateChargeByCardIdExample()
    {
        var options = new ChargeCreateOptions
        {
            Amount = 155,
            Source = new CardCreateNestedOptions
            {
                CardId = "card_Ly9v09NN2P59M0m1",
                CustomerId = "cus_jMNKVMPKnNxPVnDp"
            },
            Currency = "usd"
        };
        var charge = await Payarc.Charges.Create(options);
        Console.WriteLine("Charge Data");
        Console.WriteLine(charge);
        Console.WriteLine("Raw Data");
        Console.WriteLine(charge?.RawData);
    }

    private static async Task GetChargeById()
    {
        var charge = await Payarc.Charges.Retrieve("ch_MnBROWLXBBXnoOWL");
        Console.WriteLine("Charge Data");
        Console.WriteLine(charge);
        Console.WriteLine("Raw Data");
        Console.WriteLine(charge?.RawData);
        
    }
    
    
}