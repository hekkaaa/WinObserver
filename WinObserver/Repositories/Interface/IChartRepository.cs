﻿namespace WinObserver.Repositories.Interface
{
    public interface IChartRepository
    {
        void AddHops(string hostname);
        void AddValueLossCollection(int numberHop, double newValue);
    }
}