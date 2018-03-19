package com.company;

public class BaseGates {
    private Gate I;
    private Gate X;
    private Gate Y;
    private Gate Z;
    private Gate H;

    private Gate F;
    private Gate C;

    private Gate T;

    public BaseGates(){
        setI();
        setX();
        setY();
        setZ();
        setH();

        setC();
        setF();

        setT();
    }

    public Gate getI() { return I;}
    public Gate getX() { return X;}
    public Gate getY() { return Y;}
    public Gate getZ() { return Z;}
    public Gate getH() { return H;}

    public Gate getC() { return C;}
    public Gate getF() { return F;}

    public Gate getT() { return T;}


    private void setI(){ //
        I.setGateName("I");
        I.setArgCount(1);
        Double[][] i = {{1.0,0.0},{0.0,1.0}};
        I.setGateMatrix(i);
    }

    private void setX(){ // inverse
        X.setGateName("X");
        X.setArgCount(1);
    }
    private void setY(){ // inverse*phase
        Y.setGateName("Y");
    }
    private void setZ(){ // phase
        Z.setGateName("Z");
    }
    private void setH(){ // Hadamard
        H.setGateName("H");
    }
    private void setF(){ // Fredkin gate
        F.setGateName("F");
    }
    private void setC(){ // Cnot gate
        C.setGateName("CNOT");
    }
    private void setT(){ // Toffoli gate
        T.setGateName("T");
    }
}
