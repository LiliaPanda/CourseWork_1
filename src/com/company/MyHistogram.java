package com.company;

import java.awt.Color;
import java.awt.Dimension;
import java.awt.Font;
import java.awt.FontMetrics;
import java.awt.Graphics;
import java.awt.event.WindowAdapter;
import java.awt.event.WindowEvent;
import java.awt.event.WindowListener;
import java.util.ArrayList;

import javax.swing.JFrame;
import javax.swing.JPanel;

public class MyHistogram extends JPanel {

    private static ArrayList<Qubit> valuesList;
    private static ArrayList<String> namesList;
    private String title;

    public MyHistogram(ArrayList<Qubit> v, ArrayList<String> n, String t) {
        namesList = n;
        valuesList = v;
        title = t;
    }

    protected void paintComponent(Graphics g) {
        String[] namesArray = new String[namesList.size()];
        namesArray = namesList.toArray(namesArray);
        Qubit[] valuesArray = new Qubit[valuesList.size()];
        valuesArray = valuesList.toArray(valuesArray);

        super.paintComponent(g);
        if (valuesArray == null || valuesArray.length == 0)
            return;
        double minValue = 0;
        double maxValue = 0;
        for (int i = 0; i < valuesArray.length; i++) {
            if (minValue > valuesArray[i].getState().Ampl_1)
                minValue = valuesArray[i].getState().Ampl_1;
            if (maxValue < valuesArray[i].getState().Ampl_1)
                maxValue = valuesArray[i].getState().Ampl_1;
            if (minValue > valuesArray[i].getState().Ampl_0)
                minValue = valuesArray[i].getState().Ampl_0;
            if (maxValue < valuesArray[i].getState().Ampl_0)
                maxValue = valuesArray[i].getState().Ampl_0;
        }

        Dimension d = getSize();
        int clientWidth = d.width;
        int clientHeight = d.height;
        int barWidth = clientWidth / valuesArray.length;

        Font titleFont = new Font("LiberationSerif", Font.BOLD, 20);
        FontMetrics titleFontMetrics = g.getFontMetrics(titleFont);
        Font labelFont = new Font("SansSerif", Font.PLAIN, 10);
        FontMetrics labelFontMetrics = g.getFontMetrics(labelFont);

        int titleWidth = titleFontMetrics.stringWidth(title);
        int y = titleFontMetrics.getAscent();
        int x = (clientWidth - titleWidth) / 2;
        g.setFont(titleFont);
        g.drawString(title, x, y);

        int top = titleFontMetrics.getHeight();
        int bottom = labelFontMetrics.getHeight();
        if (maxValue == minValue)
            return;
        double scale = (clientHeight - top - bottom) /(maxValue - minValue);
        y = clientHeight - labelFontMetrics.getDescent();
        g.setFont(labelFont);

        for (int i = 0; i < valuesArray.length; i++) {
            int valueX0 = (2*i) * barWidth + 1;
            int valueY0 = top;
            int height0 = (int) (valuesArray[i].getState().Ampl_0 * scale);
            if (valuesArray[i].getState().Ampl_0 >= 0)
                valueY0 += (int) ((maxValue - valuesArray[i].getState().Ampl_0) * scale);
            else {
                valueY0 += (int) (maxValue * scale);
                height0 = -height0;
            }

            g.setColor(Color.red);
            g.fillRect(valueX0, valueY0, barWidth - 2, height0);
            g.setColor(Color.black);
            g.drawRect(valueX0, valueY0, barWidth - 2, height0);
            int labelWidth0 = labelFontMetrics.stringWidth(namesArray[i]);
            x = (2*i) * barWidth + (barWidth - labelWidth0) / 2;
            g.drawString(namesArray[i], x, y);


            int valueX1 = (2*i+1) * barWidth + 1;
            int valueY1 = top;
            int height1 = (int) (valuesArray[i].getState().Ampl_1 * scale);
            if (valuesArray[i].getState().Ampl_1 >= 0)
                valueY1 += (int) ((maxValue - valuesArray[i].getState().Ampl_1) * scale);
            else {
                valueY1 += (int) (maxValue * scale);
                height1 = -height1;
            }

            g.setColor(Color.blue);
            g.fillRect(valueX1, valueY1, barWidth - 2, height1);
            g.setColor(Color.black);
            g.drawRect(valueX1, valueY1, barWidth - 2, height1);
            int labelWidth = labelFontMetrics.stringWidth(namesArray[i]);
            x = (2*i+1) * barWidth + (barWidth - labelWidth) / 2;
            g.drawString(namesArray[i], x, y);
        }
    }

    public static void main(String[] argv) {
        JFrame f = new JFrame();
        f.setSize(1800, 225);

        f.getContentPane().add(new MyHistogram(valuesList, namesList, "title"));

        WindowListener wndCloser = new WindowAdapter() {
            public void windowClosing(WindowEvent e) {
                System.exit(0);
            }
        };
        f.addWindowListener(wndCloser);
        f.setVisible(true);
    }
}
