package com.company;

public class Qubit {
    private QState state;
    //TODO: number or index - like a place in quantum computer
    private int number;

    public QState getState() { return state; } // Measuring - after use this method qubit become unusable
    public void setState(QState st) { state = st;}
}
